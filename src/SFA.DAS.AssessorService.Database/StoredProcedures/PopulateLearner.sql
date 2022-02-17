-- Populates Learner by merging ApprovalsExtract with ILRs

CREATE PROCEDURE [dbo].[PopulateLearner]
AS
BEGIN 
   DECLARE 
		@overlaptimeIlr int = -15, -- days to allow for an overlap on ILR submissions changes
		@overlaptimeApx int = -15, -- days to allow for an overlap on Approvals changes
		@upserted int = 0;
		
	BEGIN 
		----------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------
		-- Populate Learner with latest changes from ILR or Approvals
		----------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------
		WITH LatestVersions
		AS (
			SELECT IFateReferenceNumber StandardReference, Title, Version, StandardUId, Larscode, Duration
			FROM (
				SELECT IFateReferenceNumber, Title, Version, Level, StandardUId, Larscode, ProposedTypicalDuration Duration, 
					ROW_NUMBER() OVER (PARTITION BY IFateReferenceNumber, LarsCode ORDER BY VersionMajor DESC, VersionMinor DESC) rownumber 
				FROM Standards 
				WHERE VersionApprovedForDelivery IS NOT NULL
			) sv1 WHERE rownumber = 1
		)
		,
		----------------------------------------------------------------------------------------------------------------------
		LearnerMods
		AS
		(
		-- find the recently changed learners from either data source, with an overlap to allow for missed ILR submissions
			SELECT ilrs.Uln, ilrs.StdCode FROM Ilrs 
			-- Only interested if the latest Ilrs hasn't been used already to create/updated Learner.
			LEFT JOIN Learner le2 ON le2.Uln = Ilrs.Uln AND le2.StdCode = Ilrs.StdCode 
			WHERE ilrs.LastUpdated >= (SELECT ISNULL(DATEADD(day,@overlaptimeIlr,MAX(LatestIlrs)), '01-Jan-2017') FROM Learner)
			  AND (le2.Id IS NULL OR le2.LatestIlrs < CONVERT(datetime,Ilrs.Lastupdated))
			
			UNION
			
			-- Only interested if the latest ApprovalsExtract hasn't been used already to create/updated Learner.
			SELECT ax1.Uln, TrainingCode FROM ApprovalsExtract ax1
			LEFT JOIN Learner le3 ON le3.Uln = ax1.Uln AND le3.StdCode = ax1.TrainingCode 
			WHERE ax1.LastUpdated >= (SELECT ISNULL(DATEADD(day,@overlaptimeApx,MAX(LatestApprovals)), '01-Jan-2017') FROM Learner)
			  AND (le3.Id IS NULL OR le3.LatestApprovals IS NULL OR le3.LatestApprovals < ax1.Lastupdated)
		)
		----------------------------------------------------------------------------------------------------------------------
		,
		il1
		AS (
			SELECT Ilrs.*
				  -- if could only be version 1.0 then this can be assumed as confirmed
				  ,CASE WHEN lv1.Version = '1.0' THEN '1.0' ELSE [dbo].[GetVersionFromLarsCode](LearnStartDate,Ilrs.StdCode) END Version
				  ,CASE WHEN lv1.Version = '1.0' THEN 1 ELSE 0 END VersionConfirmed
				  -- use StandardUId for version 1.0 (if appropriate) or estimate based on startdate when unknown
				  ,CASE WHEN lv1.Version = '1.0' THEN lv1.StandardUId ELSE [dbo].[GetStandardUIdFromLarsCode](LearnStartDate,Ilrs.StdCode) END StandardUId
				  ,lv1.StandardReference
				  ,lv1.Title StandardName
				  ,CASE WHEN PlannedEndDate > GETDATE() THEN EOMONTH(PlannedEndDate) ELSE EOMONTH(DATEADD(month, lv1.Duration, LearnStartDate)) END EstimatedEndDate
		   FROM Ilrs 
		   JOIN LatestVersions lv1 on lv1.LarsCode = Ilrs.StdCode
		   JOIN LearnerMods ls1 on ls1.Uln = Ilrs.Uln AND ls1.StdCode = Ilrs.StdCode  -- only include changed learners
		)
		 ,
		----------------------------------------------------------------------------------------------------------------------
		 ax1
		 AS (
		 SELECT Apx.*
				  -- leave version null if not confirmed in Approvals, as will derive from full LearnStartDate in ILR
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
				  ,LastUpdated
				  ,StopDate
				  ,PauseDate
				  ,CompletionDate
				  ,PaymentStatus
				  ,UKPRN 
				  ,LearnRefNumber
				  ,EmployerAccountId
				  ,EmployerName
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
					   ELSE 
					   -- same Provider or only one record, so use latest CreatedOn unless	
					   -- ... possibly have same Provider with duplicated data (see PRB0041207 )
					   -- Prefer the record with Active, Paused or Completed payment status, over Awaiting Approval or Cancelled
						   (CASE PaymentStatus WHEN 0 THEN 1 /* Awaiting Approval */
											   WHEN 1 THEN 0 /* Active */
											   WHEN 2 THEN 0 /* Paused */
											   WHEN 3 THEN 1 /* Cancelled */
											   WHEN 4 THEN 0 /* Completed */ 
											   ELSE 1 END)   /* N/A */
					   END
					  ,CreatedOn DESC ) as rownumber
				FROM (
					SELECT * FROM (
				-- inner query gets all records for each learner, ORDER BY CreatedOn desc - there can be many but two iterations should be sufficient
					SELECT ApprenticeshipId
					  ,FirstName
					  ,LastName
					  ,ap1.ULN
					  ,TrainingCode
					  ,TrainingCourseVersion
					  ,TrainingCourseVersionConfirmed
					  ,TrainingCourseOption
					  ,StandardUId
					  ,StartDate
					  ,EndDate
					  ,CreatedOn
					  ,UpdatedOn
					  ,LastUpdated
					  ,StopDate
					  ,PauseDate
					  ,CompletionDate
					  ,PaymentStatus
					  ,UKPRN
					  ,LearnRefNumber
					  ,EmployerAccountId
				 	  ,EmployerName
					  -- map Approvals date to ILR CompletionStatus value
					  ,CASE WHEN StopDate IS NOT NULL THEN 3
							WHEN PauseDate IS NOT NULL THEN 6
							WHEN CompletionDate IS NOT NULL THEN 2 
							ELSE (CASE WHEN PaymentStatus = 1 THEN 1 ELSE 0 END) END CompletionStatus 
					  ,LAG(UKPRN, 1,0) OVER (PARTITION BY ap1.ULN, TrainingCode ORDER BY CreatedOn ) AS UKPRN_1
					  ,LAG(StartDate, 1,0) OVER (PARTITION BY ap1.ULN, TrainingCode ORDER BY CreatedOn ) AS StartDate_1
					  ,LAG(EndDate, 1,0) OVER (PARTITION BY ap1.ULN, TrainingCode ORDER BY CreatedOn ) AS EndDate_1
					  ,LAG(CONVERT(datetime,StopDate), 1,0) OVER (PARTITION BY ap1.ULN, TrainingCode ORDER BY CreatedOn ) AS StopDate_1
					FROM ApprovalsExtract ap1
 					JOIN LearnerMods ls1 on ls1.Uln = ap1.Uln AND ls1.StdCode = ap1.TrainingCode  -- only include changed learners
					) ax2
					WHERE 1=1
					  AND NOT (UKPRN_1 !=0 AND StopDate IS NOT NULL AND EOMONTH(StopDate) = EOMONTH(StartDate) AND PaymentStatus = 3) -- cancelled, not started, effectively deleted and not the only record
				) ab2
			) ab3 WHERE rownumber = 1
			) Apx 
		)
		----------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------

		MERGE INTO Learner lm1
		USING 
		(
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
				il1.LearnRefNumber, 
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
				-- If Version set in Approvals use this, otherwise take from Startdate in ILR
				ISNULL(ax1.Version,il1.Version) Version,
				CASE WHEN ax1.Version IS NOT NULL THEN ax1.VersionConfirmed ELSE il1.VersionConfirmed END VersionConfirmed,
				ax1.TrainingCourseOption CourseOption,
				-- If StandardUId set in Approvals use this, otherwise take from Startdate in ILR
				ISNULL(ax1.StandardUId,il1.StandardUId) StandardUid,
				il1.StandardReference,
				il1.StandardName,
				CASE WHEN ax1.LastUpdated > il1.LastUpdated THEN ax1.LastUpdated ELSE il1.LastUpdated END LastUpdated, 
				il1.EstimatedEndDate,
				ax1.StopDate ApprovalsStopDate,
				ax1.PauseDate ApprovalsPauseDate,
				ax1.CompletionDate ApprovalsCompletionDate,
				ax1.PaymentStatus ApprovalsPaymentStatus,
				il1.LastUpdated LatestIlrs,
				ax1.LastUpdated LatestApprovals,
				ax1.EmployerAccountId,
				ax1.EmployerName
		  FROM ax1 
		  JOIN il1 ON ax1.ULN = il1.ULN  AND il1.StdCode = ax1.TrainingCode	
		  WHERE il1.FundingModel != 99
			AND ax1.CompletionStatus != 0
		  
		----------------------------------------------------------------------------------------------------------------------
		-- just ILrs (trimmed Expired and Lapsed ILR records!)  
		-- or FM99 Ilr to override Approval as data source
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
				il1.EstimatedEndDate,
				null ApprovalsStopDate,
				null ApprovalsPauseDate,
				null ApprovalsCompletionDate,
				null ApprovalsPaymentStatus, 
				il1.LastUpdated LatestIlrs,
				null LatestApprovals,
				null EmployerAccountId,
				null EmployerName
		  FROM il1 
		  LEFT JOIN ax1 ON ax1.ULN = il1.ULN  AND il1.StdCode = ax1.TrainingCode
		  WHERE (il1.FundingModel = 99 OR ax1.ULN IS NULL)
		) upd
		ON (lm1.uln = upd.uln AND lm1.StdCode = upd.StdCode)

		WHEN MATCHED THEN UPDATE
		SET 
			 lm1.GivenNames = upd.GivenNames,
			 lm1.FamilyName = upd.FamilyName,
			 lm1.UkPrn = upd.UkPrn,
			 lm1.LearnStartDate = upd.LearnStartDate,
			 lm1.EpaOrgId = upd.EpaOrgId,
			 lm1.FundingModel = upd.FundingModel,
			 lm1.ApprenticeshipId = upd.ApprenticeshipId,
			 lm1.Source = upd.Source,
			 lm1.LearnRefNumber = upd.LearnRefNumber,
			 lm1.CompletionStatus = upd.CompletionStatus,
			 lm1.PlannedEndDate = upd.PlannedEndDate,
			 lm1.DelLocPostCode = upd.DelLocPostCode,
			 lm1.LearnActEndDate = upd.LearnActEndDate,
			 lm1.WithdrawReason = upd.WithdrawReason,
			 lm1.Outcome = upd.Outcome,
			 lm1.AchDate = upd.AchDate,
			 lm1.OutGrade = upd.OutGrade,
			 lm1.Version = upd.Version,
			 lm1.VersionConfirmed = upd.VersionConfirmed,
			 lm1.CourseOption = upd.CourseOption,
			 lm1.StandardUId = upd.StandardUId,
			 lm1.StandardReference = upd.StandardReference,
			 lm1.StandardName = upd.StandardName,
			 lm1.LastUpdated = upd.LastUpdated,
			 lm1.EstimatedEndDate = upd.EstimatedEndDate,
			 lm1.ApprovalsStopDate = upd.ApprovalsStopDate,
			 lm1.ApprovalsPauseDate = upd.ApprovalsPauseDate,
			 lm1.ApprovalsCompletionDate = upd.ApprovalsCompletionDate,
			 lm1.ApprovalsPaymentStatus = upd.ApprovalsPaymentStatus,
			 lm1.LatestIlrs = upd.LatestIlrs,
			 lm1.LatestApprovals = upd.LatestApprovals,
			 lm1.EmployerAccountId = upd.EmployerAccountId,
			 lm1.EmployerName = upd.EmployerName
		WHEN NOT MATCHED THEN
		INSERT (Id, Uln, GivenNames, FamilyName, UkPrn, StdCode, LearnStartDate, EpaOrgId, FundingModel, ApprenticeshipId, 
				Source, LearnRefNumber, CompletionStatus, PlannedEndDate, DelLocPostCode, LearnActEndDate, WithdrawReason, 
				Outcome, AchDate, OutGrade, Version, VersionConfirmed, CourseOption, StandardUId, StandardReference, StandardName, 
				LastUpdated, EstimatedEndDate, ApprovalsStopDate, ApprovalsPauseDate, ApprovalsCompletionDate, ApprovalsPaymentStatus,
				LatestIlrs, LatestApprovals, EmployerAccountId, EmployerName)
		VALUES (upd.Id, upd.Uln, upd.GivenNames, upd.FamilyName, upd.UkPrn, upd.StdCode, upd.LearnStartDate, upd.EpaOrgId, upd.FundingModel, upd.ApprenticeshipId,
				upd.Source, upd.LearnRefNumber, upd.CompletionStatus, upd.PlannedEndDate, upd.DelLocPostCode, upd.LearnActEndDate, upd.WithdrawReason,
				upd.Outcome, upd.AchDate, upd.OutGrade, upd.Version, upd.VersionConfirmed, upd.CourseOption, upd.StandardUId, upd.StandardReference, upd.StandardName,
				upd.LastUpdated, upd.EstimatedEndDate, upd.ApprovalsStopDate, upd.ApprovalsPauseDate, upd.ApprovalsCompletionDate, upd.ApprovalsPaymentStatus,
				upd.LatestIlrs, upd.LatestApprovals, upd.EmployerAccountId, upd.EmployerName);

		SET @upserted = @@ROWCOUNT;

	END
SELECT @upserted
END;
GO