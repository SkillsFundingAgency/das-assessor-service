-- Populates Learner by merging ApprovalsExtract with ILRs
CREATE PROCEDURE [dbo].[PopulateLearner]
 		@overlaptimeIlr int = -15,  -- days to allow for an overlap on ILR submissions changes
		@overlaptimeApx int = -15,  -- days to allow for an overlap on Approvals changes
		@transferWindow int = -60,  -- days to allow for Transfer predication, after which it is voided (as nothing happened)
		@reset Int = 0              -- set to 1 to do a full reset of learner

AS
BEGIN
   DECLARE
		@upserted int = 0;
		
	BEGIN

		----------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------
		-- Reset predicted Transfers where this has exceeded the transfer window
		-- Will be resetting IsTransfer flag after the transfer window
		-- 2  'Learner has transferred to another provider'
		-- 7  'Learner has transferred between providers due to intervention by or with the written agreement of the ESFA'
		-- 41 'Learner has transferred to another provider to undertake learning that meets a specific government strategy'
		-- 47 'Learner has transferred to another provider due to merger'
		----------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------
		UPDATE Learner
		SET IsTransfer = 0
		WHERE IsTransfer = 1
		AND DateTransferIdentified < DATEADD(day,@transferWindow,GETUTCDATE());

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
			WHERE @reset =1 OR
			 (ilrs.LastUpdated >= (SELECT ISNULL(DATEADD(day,@overlaptimeIlr,MAX(LatestIlrs)), '01-Jan-2017') FROM Learner)
			  AND (le2.Id IS NULL OR le2.LatestIlrs < CONVERT(datetime,Ilrs.Lastupdated)) )

			UNION
			
			-- Only interested if the latest ApprovalsExtract hasn't been used already to create/updated Learner.
			SELECT ax1.Uln, TrainingCode FROM ApprovalsExtract ax1
			LEFT JOIN Learner le3 ON le3.Uln = ax1.Uln AND le3.StdCode = ax1.TrainingCode
			WHERE @reset =1 OR
			 (ax1.LastUpdated >= (SELECT ISNULL(DATEADD(day,@overlaptimeApx,MAX(LatestApprovals)), '01-Jan-2017') FROM Learner)
			  AND (le3.Id IS NULL OR le3.LatestApprovals IS NULL OR le3.LatestApprovals < ax1.Lastupdated) )
			
			UNION
			
			-- need to check if predicted Transfers have now been resolved
			SELECT Uln, StdCode FROM Learner 
			WHERE IsTransfer = 1 
			AND DateTransferIdentified >= DATEADD(day,@transferWindow,GETUTCDATE())
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
				  -- Is this showing Transfer to other provider ?
				  ,CASE WHEN (CompletionStatus = 3 AND WithdrawReason IN (2, 7, 41 ,47) )
						THEN 1
						ELSE 0 END IsTransfer
		
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
				  -- Is there a previous & different Training Provider
				  ,CASE WHEN Check_Previous_UKPRN != 0 AND Check_Previous_UKPRN != UKPRN
						THEN Check_Previous_UKPRN
						ELSE NULL END Previous_UKPRN
				  -- Is there a previous Apprenticeship for a different Training Provider
				  ,CASE WHEN Check_Previous_UKPRN != 0 AND Check_Previous_UKPRN != UKPRN AND ApprenticeshipId_1 != 0
						THEN ApprenticeshipId_1
						ELSE NULL END Previous_ApprenticeshipId
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
				  ,LEAD(UKPRN, 1,0) OVER (PARTITION BY ULN, TrainingCode ORDER BY rownumber ) AS Check_Previous_UKPRN
				  ,LEAD(ApprenticeshipId, 1,0) OVER (PARTITION BY ULN, TrainingCode ORDER BY rownumber ) AS ApprenticeshipId_1
				  ,rownumber
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
					  -- Previous Apprenticeship record
					  ,LAG(UKPRN, 1,0) OVER (PARTITION BY ap1.ULN, TrainingCode ORDER BY CreatedOn ) AS UKPRN_1
					  ,LAG(CONVERT(datetime,StartDate), 1,0) OVER (PARTITION BY ap1.ULN, TrainingCode ORDER BY CreatedOn ) AS StartDate_1
					  ,LAG(CONVERT(datetime,EndDate), 1,0) OVER (PARTITION BY ap1.ULN, TrainingCode ORDER BY CreatedOn ) AS EndDate_1
					  ,LAG(CONVERT(datetime,StopDate), 1,0) OVER (PARTITION BY ap1.ULN, TrainingCode ORDER BY CreatedOn ) AS StopDate_1
					FROM ApprovalsExtract ap1
 					JOIN LearnerMods ls1 on ls1.Uln = ap1.Uln AND ls1.StdCode = ap1.TrainingCode  -- only include changed learners
					) ax2
					WHERE 1=1
					  AND NOT (UKPRN_1 !=0 AND StopDate IS NOT NULL AND EOMONTH(StopDate) = EOMONTH(StartDate) AND PaymentStatus = 3) -- cancelled, not started, effectively deleted and not the only record
				) ab2
			) ab3 WHERE rownumber IN (1,2)
			) Apx WHERE rownumber = 1
		)
		----------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------
		MERGE INTO Learner lm1
		USING
		( 
		----------------------------------------------------------------------------------------------------------------------
		-- using ILR with Approvals Extract for some key fields that can be updated in Approvals
		----------------------------------------------------------------------------------------------------------------------
		-- QF-826 - handle temp state where Approvals withdrawals shows move to new Training Provider
		----------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------
		-- Merge ILR and Approvals, where UKPRN will decide if to use latest or previous (if one) Approval record
		----------------------------------------------------------------------------------------------------------------------
		SELECT  il1.Id,
				il1.Uln,
				CASE WHEN ISNULL(ax2.LastUpdated,ax1.LastUpdated) < il1.LastUpdated
					 THEN il1.GivenNames
					 ELSE ISNULL(ax2.FirstName,ax1.FirstName) END GivenNames,
				CASE WHEN ISNULL(ax2.LastUpdated,ax1.LastUpdated) < il1.LastUpdated
					 THEN il1.FamilyName
					 ELSE ax1.LastName END FamilyName,
				il1.UkPrn,
				il1.StdCode,
				il1.LearnStartDate,
				il1.EpaOrgId,
				il1.FundingModel,
				ISNULL(ax2.ApprenticeshipId,ax1.ApprenticeshipId) ApprenticeshipId,
				Source+'+App' Source,
				il1.LearnRefNumber,
				-- Approval Stop or Pause to overridde Active ILR - otherwise use ILR
				CASE WHEN il1.CompletionStatus != 1
					 THEN il1.CompletionStatus
					 WHEN ax2.ApprenticeshipId IS NOT NULL AND (ax2.StopDate IS NOT NULL OR ax2.PauseDate IS NOT NULL)
					 THEN ax2.CompletionStatus
					 WHEN ax1.StopDate IS NOT NULL OR ax1.PauseDate IS NOT NULL
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
				CASE WHEN ax2.ApprenticeshipId IS NOT NULL
					 THEN ISNULL(ax2.Version,il1.Version)
					 ELSE ISNULL(ax1.Version,il1.Version) END Version,
				CASE WHEN ax2.ApprenticeshipId IS NOT NULL AND ax2.Version IS NOT NULL
					 THEN ax2.VersionConfirmed
					 WHEN ax1.Version IS NOT NULL
					 THEN ax1.VersionConfirmed
					 ELSE il1.VersionConfirmed END VersionConfirmed,
				ISNULL(ax2.TrainingCourseOption,ax1.TrainingCourseOption) CourseOption,
				-- If StandardUId set in Approvals use this, otherwise take from Startdate in ILR
				ISNULL(ax2.StandardUId,ISNULL(ax1.StandardUId,il1.StandardUId)) StandardUid,
				il1.StandardReference,
				il1.StandardName,
				CASE WHEN ax2.ApprenticeshipId IS NOT NULL AND ax2.LastUpdated > il1.LastUpdated
					 THEN ax2.LastUpdated
					 WHEN ax1.LastUpdated > il1.LastUpdated
					 THEN ax1.LastUpdated
					 ELSE il1.LastUpdated END LastUpdated,
				il1.EstimatedEndDate,
				CASE WHEN ax2.ApprenticeshipId IS NOT NULL
					 THEN ax2.StopDate
					 ELSE ax1.StopDate END ApprovalsStopDate,
				CASE WHEN ax2.ApprenticeshipId IS NOT NULL
					 THEN ax2.PauseDate
					 ELSE ax1.PauseDate END ApprovalsPauseDate,
				CASE WHEN ax2.ApprenticeshipId IS NOT NULL
					 THEN ax2.CompletionDate
					 ELSE ax1.CompletionDate END ApprovalsCompletionDate,
				ISNULL(ax2.PaymentStatus,ax1.PaymentStatus) ApprovalsPaymentStatus,
				il1.LastUpdated LatestIlrs,
				ISNULL(ax2.LastUpdated,ax1.LastUpdated) LatestApprovals,
				CASE WHEN ax2.ApprenticeshipId IS NOT NULL
					 THEN ax2.EmployerAccountId
					 ELSE ax1.EmployerAccountId END EmployerAccountId,
				CASE WHEN ax2.ApprenticeshipId IS NOT NULL
					 THEN ax2.EmployerName
					 ELSE ax1.EmployerName END EmployerName,
				-- If UKPRNs don't match for latest values this implies a transfer
				-- Can have explict Transfer code from TP where UKPRNs do match
				-- Also, where the Employer has Stopped and TP has Paused this is likely to be a forthcoming Transfer
				-- Or where Employer has Stopped/Paused (in past 2(?) months) and TP is Active
				CASE WHEN il1.UKPRN != ax1.UKPRN OR il1.IsTransfer = 1
					 THEN 1
					 WHEN il1.CompletionStatus = 6 AND ax1.CompletionStatus = 3
					 THEN 1
					 WHEN il1.CompletionStatus = 1 AND ax1.CompletionStatus IN (3,6) AND
						  ISNULL(ax1.StopDate,ax1.PauseDate) >= DATEADD(day,@transferWindow,GETUTCDATE()) AND
						  il1.LastUpdated >= DATEADD(day,@transferWindow,GETUTCDATE())
					 THEN 1
					 ELSE 0
					 END IsTransfer,
				CASE WHEN il1.IsTransfer = 1
					 THEN il1.LastUpdated
					 WHEN il1.UKPRN != ax1.UKPRN OR (il1.CompletionStatus = 6 AND ax1.CompletionStatus = 3)
					 THEN (CASE WHEN il1.LastUpdated > ax1.LastUpdated THEN il1.LastUpdated ELSE ax1.LastUpdated END)
					 WHEN il1.CompletionStatus = 1 AND ax1.CompletionStatus IN (3,6) AND
						  ISNULL(ax1.StopDate,ax1.PauseDate) >= DATEADD(day,@transferWindow,GETUTCDATE()) AND
						  il1.LastUpdated >= DATEADD(day,@transferWindow,GETUTCDATE())
					 THEN ax1.LastUpdated
					 ELSE NULL END DateTransferIdentified
		  FROM ax1
		  JOIN il1 ON ax1.ULN = il1.ULN  AND il1.StdCode = ax1.TrainingCode	
		  -- join in previous apprenticeship if there was one and UKPRN differs and matches ILR
		  LEFT JOIN (SELECT * FROM
						 (SELECT *
						  -- leave version null if not confirmed in Approvals, as will derive from full LearnStartDate in ILR
						  ,CASE WHEN TrainingCourseVersionConfirmed = 1 THEN TrainingCourseVersion ELSE null END Version
						  ,TrainingCourseVersionConfirmed VersionConfirmed
						  ,CASE WHEN StopDate IS NOT NULL THEN 3
								WHEN PauseDate IS NOT NULL THEN 6
								WHEN CompletionDate IS NOT NULL THEN 2
								ELSE (CASE WHEN PaymentStatus = 1 THEN 1 ELSE 0 END) END CompletionStatus
						 FROM ApprovalsExtract) ab2 WHERE CompletionStatus != 0
					 ) ax2 on ax2.ApprenticeshipId = ax1.Previous_ApprenticeshipId AND ax2.UKPRN = il1.UKPRN
		  -- include ILR & Approvals only where match can be found on UKPRN for latest or previous Approvals
		  -- except where privately funded or only Approvals record is in unknowm status
		  -- otherwise take ILR only
		  WHERE il1.FundingModel != 99
			AND ax1.CompletionStatus != 0
			AND (il1.UKPRN = ax1.UKPRN OR ax2.ApprenticeshipId IS NOT NULL)
		
		----------------------------------------------------------------------------------------------------------------------
		-- just ILrs (trimmed Expired and Lapsed ILR records!)
		-- or FM99 Ilr to override Approval as data source
		-- or a Transfer
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
				null EmployerName,
				CASE WHEN (ax1.UKPRN IS NOT NULL AND il1.UKPRN != ax1.UKPRN)
					 THEN 1
					 ELSE il1.IsTransfer END IsTransfer,
				CASE WHEN il1.IsTransfer = 1
					 THEN il1.LastUpdated
					 WHEN (ax1.UKPRN IS NOT NULL AND il1.UKPRN != ax1.UKPRN)
					 THEN il1.LastUpdated
					 ELSE NULL END DateTransferIdentified
		  FROM il1
		  LEFT JOIN ax1 ON ax1.ULN = il1.ULN  AND il1.StdCode = ax1.TrainingCode
		  WHERE il1.FundingModel = 99 OR ax1.UKPRN IS NULL OR
				(ax1.UKPRN IS NOT NULL AND il1.UKPRN != ax1.UKPRN AND (ax1.Previous_UKPRN IS NULL OR il1.UKPRN != ax1.Previous_UKPRN))
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
			 -- Set Employer Account Name and ID only if it comes from Approvals, otherwise keep existing value.
			 lm1.EmployerAccountId = CASE WHEN upd.LatestApprovals IS NULL THEN lm1.EmployerAccountId ELSE upd.EmployerAccountId END,
			 lm1.EmployerName = CASE WHEN upd.LatestApprovals IS NULL THEN lm1.EmployerName ELSE upd.EmployerName END,
			 -- Set IsTransfer, but do not reset if already has been expired
			 lm1.IsTransfer = CASE WHEN lm1.DateTransferIdentified IS NULL 
								   THEN upd.IsTransfer
								   WHEN lm1.DateTransferIdentified >= DATEADD(day,@transferWindow,GETUTCDATE()) 
								   THEN upd.IsTransfer
								   ELSE lm1.IsTransfer END,
			 lm1.DateTransferIdentified = CASE WHEN lm1.DateTransferIdentified IS NULL AND lm1.IsTransfer = 0 
											   THEN NULL
											   ELSE ISNULL(lm1.DateTransferIdentified,upd.DateTransferIdentified) END
			
		WHEN NOT MATCHED THEN
		INSERT (Id, Uln, GivenNames, FamilyName, UkPrn, StdCode, LearnStartDate, EpaOrgId, FundingModel, ApprenticeshipId,
				Source, LearnRefNumber, CompletionStatus, PlannedEndDate, DelLocPostCode, LearnActEndDate, WithdrawReason,
				Outcome, AchDate, OutGrade, Version, VersionConfirmed, CourseOption, StandardUId, StandardReference, StandardName,
				LastUpdated, EstimatedEndDate, ApprovalsStopDate, ApprovalsPauseDate, ApprovalsCompletionDate, ApprovalsPaymentStatus,
				LatestIlrs, LatestApprovals, EmployerAccountId, EmployerName,
				IsTransfer, DateTransferIdentified)
		VALUES (upd.Id, upd.Uln, upd.GivenNames, upd.FamilyName, upd.UkPrn, upd.StdCode, upd.LearnStartDate, upd.EpaOrgId, upd.FundingModel, upd.ApprenticeshipId,
				upd.Source, upd.LearnRefNumber, upd.CompletionStatus, upd.PlannedEndDate, upd.DelLocPostCode, upd.LearnActEndDate, upd.WithdrawReason,
				upd.Outcome, upd.AchDate, upd.OutGrade, upd.Version, upd.VersionConfirmed, upd.CourseOption, upd.StandardUId, upd.StandardReference, upd.StandardName,
				upd.LastUpdated, upd.EstimatedEndDate, upd.ApprovalsStopDate, upd.ApprovalsPauseDate, upd.ApprovalsCompletionDate, upd.ApprovalsPaymentStatus,
				upd.LatestIlrs, upd.LatestApprovals, upd.EmployerAccountId, upd.EmployerName,
				upd.IsTransfer, upd.DateTransferIdentified);

		SET @upserted = @@ROWCOUNT;

	END
SELECT @upserted
END;
GO
