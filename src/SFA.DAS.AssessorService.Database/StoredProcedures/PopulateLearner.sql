-- Populates Learner by merging ApprovalsExtract with ILRs
CREATE PROCEDURE [dbo].[PopulateLearner]
AS
BEGIN
   DECLARE 
		@expiretime int = -12,  -- months to allow for overrun after planned/estimated end date before EPA should have been done
		@lapsedtime int = -14;  -- months to allow for delay in submitting ILRs (should not be greater than 14)
		
   BEGIN TRANSACTION;
   SAVE TRANSACTION Updatelearner;
   
   BEGIN TRY
   
		-- clear learner data in a transaction 
		DELETE FROM Learner WHERE Id IS NOT NULL;
		
		-- re-populate
		----------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------
		WITH LatestVersions
		AS (
			SELECT IFateReferenceNumber StandardReference, Title, Version, StandardUId, Larscode, Duration
			  FROM (
			   SELECT IFateReferenceNumber, Title, Version, Level, StandardUId, Larscode, ProposedTypicalDuration Duration, 
					  ROW_NUMBER() OVER (PARTITION BY IFateReferenceNumber ORDER BY dbo.ExpandedVersion(Version) DESC) rownumber 
				 FROM Standards
				WHERE VersionApprovedForDelivery IS NOT NULL
			) sv1 WHERE rownumber = 1
		)
		,
		----------------------------------------------------------------------------------------------------------------------
		il1
		AS (
			SELECT *
			-- check for ILR records that are for Apprenticeships that have significantly overrun the end date
			,CASE WHEN EstimatedEndDate < dateadd(month, @expiretime, GETDATE()) THEN 1 ELSE 0 END Expired
			-- check for "Continuing" ILR records that have not been updated for a long time - they should be updated every month.
			,CASE WHEN CompletionStatus = 1 AND LastUpdated < DATEADD(month, @lapsedtime, GETDATE()) THEN 1 ELSE 0 END Lapsed
			FROM (
				SELECT Ilrs.*, ISNULL(UpdatedAt,CreatedAt) LastUpdated
					  -- if could only be version 1.0 then this can be assumed as confirmed
					  ,CASE WHEN lv1.Version = '1.0' THEN '1.0' ELSE [dbo].[GetVersionFromLarsCode](LearnStartDate,StdCode) END Version
					  ,CASE WHEN lv1.Version = '1.0' THEN 1 ELSE 0 END VersionConfirmed
					  -- use StandardUId for version 1.0 (if appropriate) or estimate based on startdate when unknown
					  ,CASE WHEN lv1.Version = '1.0' THEN lv1.StandardUId ELSE [dbo].[GetStandardUidFromLarsCode](LearnStartDate,StdCode) END StandardUId
					  ,lv1.StandardReference
					  ,lv1.Title StandardName
					  ,CASE WHEN PlannedEndDate > GETDATE() THEN EOMONTH(PlannedEndDate) ELSE EOMONTH(DATEADD(month, lv1.Duration, LearnStartDate)) END EstimatedEndDate
			   FROM Ilrs JOIN LatestVersions lv1 on lv1.LarsCode = Ilrs.StdCode
			) il2
		)
		 ,
		----------------------------------------------------------------------------------------------------------------------
		 ax1
		 AS (
		 SELECT Apx.*, ISNULL(UpdatedOn,CreatedOn) LastUpdated
				  -- leave version null if not confirmed in Approvals, as will derive from full LearnStartDate in ILRs or EOMonth of StartDate in Approvals
				  ,CASE WHEN TrainingCourseVersionConfirmed = 1 THEN TrainingCourseVersion ELSE null END Version
				  ,TrainingCourseVersionConfirmed VersionConfirmed
			FROM (
			SELECT ApprenticeshipId
				  ,FirstName
				  ,LastName
				  ,ULN
				  ,TrainingCode
				  ,TrainingCourseVersion
				  ,TrainingCourseVersionConfirmed
				  ,TrainingCourseOption
				  ,StandardUId
				  ,StartDate
				  ,EndDate
				  ,CreatedOn
				  ,UpdatedOn
				  ,StopDate
				  ,PauseDate
				  ,CompletionDate
				  ,StandardReference
				  ,UKPRN 
				  ,LearnRefNumber
				  ,CompletionStatus
			FROM (
				SELECT *
                    -- apply rules to determine the chronologically latest Apprenticeship record
					,ROW_NUMBER() OVER (PARTITION BY ULN, TrainingCode ORDER BY 
					   CASE 
					   -- if StopDate of previous record is before Startdate of current record then prefer current record
					   WHEN StopDate_1 IS NOT NULL AND EOMONTH(StartDate) >= EOMONTH(StopDate_1) THEN 0 
					   -- if StopDate of current record is before Startdate of previous record then prefer previous record
					   WHEN StopDate IS NOT NULL AND EOMONTH(StopDate) <= EOMONTH(StartDate_1) THEN 1 
					   -- if both have StopDate then most recent one is the likely latest record
					   WHEN StopDate IS NOT NULL AND StopDate_1 IS NOT NULL AND EOMONTH(StopDate_1) > EOMONTH(Stopdate) THEN 1 
					   ELSE 0 END
					  ,CASE 
					   WHEN UKPRN_1 != 0 AND UKPRN != UKPRN_1 THEN
					   -- different provider, watch-out for retro-Approvals for earlier training period
						   (CASE WHEN StartDate < StartDate_1 AND EndDate < Enddate_1 THEN 1 ELSE 0 END)
					   -- same Provider or only one record, so use latest CreatedOn unless	
					   ELSE 0 END 
					  ,CreatedOn DESC ) as rownumber
				FROM (
				-- inner query gets all records for each learner, ORDER BY CreatedOn desc - there can be many but two iterations should be sufficient
					SELECT ApprenticeshipId
					  ,FirstName
					  ,LastName
					  ,ULN
					  ,TrainingCode
					  ,TrainingCourseVersion
					  ,TrainingCourseVersionConfirmed
					  ,TrainingCourseOption
					  ,StandardUId
					  ,StartDate
					  ,EndDate
					  ,CreatedOn
					  ,UpdatedOn
					  ,StopDate
					  ,PauseDate
					  ,CompletionDate
					  ,StandardReference
					  ,UKPRN
					  ,LearnRefNumber
					  -- map Approvals date to ILR CompletionStatus value
					  ,CASE WHEN StopDate IS NOT NULL THEN 3
							WHEN PauseDate IS NOT NULL THEN 6
							WHEN CompletionDate IS NOT NULL THEN 2 
							ELSE (CASE WHEN PaymentStatus = 1 THEN 1 ELSE 0 END) END CompletionStatus 
					  ,LAG(UKPRN, 1,0) OVER (PARTITION BY ULN, TrainingCode ORDER BY CreatedOn ) AS UKPRN_1
					  ,LAG(StartDate, 1,0) OVER (PARTITION BY ULN, TrainingCode ORDER BY CreatedOn ) AS StartDate_1
					  ,LAG(EndDate, 1,0) OVER (PARTITION BY ULN, TrainingCode ORDER BY CreatedOn ) AS EndDate_1
					  ,LAG(CONVERT(datetime,StopDate), 1,0) OVER (PARTITION BY ULN, TrainingCode ORDER BY CreatedOn ) AS StopDate_1
					FROM ApprovalsExtract ap1
					WHERE 1=1
					  AND NOT (StopDate IS NOT NULL AND EOMONTH(StopDate) = EOMONTH(StartDate) AND PaymentStatus = 3) -- cancelled, not started, effectively deleted
				) ab2
			) ab3 WHERE rownumber = 1
			) Apx  WHERE CompletionStatus != 0  -- ignore where Apprenticeship is "PendingApproval"
		)
		----------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------
		INSERT INTO Learner (Id, Uln, GivenNames, FamilyName, UkPrn, StdCode, LearnStartDate, EpaOrgId, FundingModel, ApprenticeshipId, 
		Source, LearnRefNumber, CompletionStatus, PlannedEndDate, DelLocPostCode, LearnActEndDate, WithdrawReason, 
		Outcome, AchDate, OutGrade, Version, VersionConfirmed, CourseOption, StandardUId, StandardReference, StandardName, LastUpdated, EstimatedEndDate )

		----------------------------------------------------------------------------------------------------------------------
		-- using ILR with Approvals Extract for some key fields that can be updated in Approvals 
		----------------------------------------------------------------------------------------------------------------------
		SELECT  il1.Id, 
				il1.Uln, 
				CASE WHEN ax1.LastUpdated < il1.LastUpdated THEN il1.GivenNames ELSE ax1.FirstName END GivenNames, 
				CASE WHEN ax1.LastUpdated < il1.LastUpdated THEN il1.FamilyName ELSE ax1.LastName END FamilyName, 
				CASE WHEN ax1.LastUpdated < il1.LastUpdated THEN il1.UkPrn ELSE ax1.UkPrn END UkPrn, 
				il1.StdCode, 
				il1.LearnStartDate, 
				il1.EpaOrgId,	 
				il1.FundingModel,
				ax1.ApprenticeshipId, 
				Source+'+App' Source,
				CASE WHEN ax1.LastUpdated < il1.LastUpdated THEN il1.LearnRefNumber ELSE ax1.LearnRefNumber END LearnRefNumber, 
				-- Approval Stop or Pause to overridde Active ILR - otherwise use ILR
				CASE WHEN (ax1.StopDate IS NOT NULL OR ax1.PauseDate IS NOT NULL) AND il1.CompletionStatus = 1 
					 THEN ax1.CompletionStatus 
					 ELSE il1.CompletionStatus END CompletionStatus,
				il1.PlannedEndDate,
				il1.DelLocPostCode,
				il1.LearnActEndDate,
				il1.WithdrawReason,
				il1.Outcome,
				il1.AchDate,
				il1.OutGrade,	
				ISNULL(ax1.Version,il1.Version) Version,
				CASE WHEN ax1.Version IS NOT NULL THEN ax1.VersionConfirmed ELSE il1.VersionConfirmed END VersionConfirmed,
				ax1.TrainingCourseOption CourseOption,
				ISNULL(ax1.StandardUId,il1.StandardUId) StandardUid,
				ISNULL(ax1.StandardReference,il1.StandardReference) StandardReference,
				il1.StandardName,
				CASE WHEN ax1.LastUpdated > il1.LastUpdated THEN ax1.LastUpdated ELSE il1.LastUpdated END LastUpdated, 
				il1.EstimatedEndDate
		  FROM ax1 
		  JOIN il1 ON ax1.ULN = il1.ULN  AND il1.StdCode = ax1.TrainingCode	
		  WHERE NOT ( (ax1.StopDate IS NOT NULL OR ax1.PauseDate IS NOT NULL) AND il1.FundingModel = 99 )
		  
		----------------------------------------------------------------------------------------------------------------------
		-- just ILrs (trimmed Expired and Lapsed ILR records!)  
		-- or FM99 Ilr to override Stopped/Paused Approval as data source
		----------------------------------------------------------------------------------------------------------------------
		UNION ALL
		SELECT  il1.Id, 
				il1.Uln, 
				il1.GivenNames, 
				il1.FamilyName, 
				il1.UkPrn, 
				il1.StdCode, 
				il1.LearnStartDate, 
				il1.EpaOrgId,
				il1.FundingModel,
				null ApprenticeshipId,
				il1.Source,
				il1.LearnRefNumber,
				il1.CompletionStatus,
				il1.PlannedEndDate,
				il1.DelLocPostCode,
				il1.LearnActEndDate,
				il1.WithdrawReason,
				il1.Outcome,
				il1.AchDate,
				il1.OutGrade,
				il1.Version,
				il1.VersionConfirmed,
				null CourseOption,
				il1.StandardUId,
				il1.StandardReference,
				il1.StandardName,
				il1.LastUpdated,
				il1.EstimatedEndDate
		  FROM il1 
		  LEFT JOIN ax1 ON ax1.ULN = il1.ULN  AND il1.StdCode = ax1.TrainingCode
		  WHERE (ax1.ULN IS NULL OR ( (ax1.StopDate IS NOT NULL OR ax1.PauseDate IS NOT NULL) AND il1.FundingModel = 99) )
		  AND Lapsed = 0 
		  AND Expired = 0
		  
   
		COMMIT TRANSACTION 
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
		-- rollback Learner Update
			ROLLBACK TRANSACTION Updatelearner; 
		END
	END CATCH
RETURN 0
END;
GO   
