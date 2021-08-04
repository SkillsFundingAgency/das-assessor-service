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
        DELETE FROM Learner;
        
        -- re-populate
        ----------------------------------------------------------------------------------------------------------------------
        ----------------------------------------------------------------------------------------------------------------------
        WITH LatestVersions
        AS
        (
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
        AS
        ( SELECT *
            -- check for ILR records that are for Apprenticeships that have significantly overrun the end date
            ,CASE WHEN EstimatedEndDate < dateadd(month, @expiretime, GETDATE()) THEN 1 ELSE 0 END Expired
            -- check for "Continuing" ILR records that have not been updated for a long time - they should be updated every month.
            ,CASE WHEN CompletionStatus = 1 AND LastUpdated < DATEADD(month, @lapsedtime, GETDATE()) THEN 1 ELSE 0 END Lapsed
           FROM (
              SELECT Ilrs.*, ISNULL(UpdatedAt,CreatedAt) LastUpdated
                      ,CASE WHEN lv1.Version = '1.0' THEN lv1.version ELSE null END Version
                      ,CASE WHEN lv1.Version = '1.0' THEN lv1.StandardUId ELSE null END StandardUId
                      ,lv1.StandardReference
					  ,lv1.Title StandardName
                      ,CASE WHEN PlannedEndDate > GETDATE() THEN EOMONTH(PlannedEndDate) ELSE EOMONTH(DATEADD(month, lv1.Duration, LearnStartDate)) END EstimatedEndDate
               FROM Ilrs JOIN LatestVersions lv1 on lv1.LarsCode = Ilrs.StdCode
            ) il2
        )
         ,
        ----------------------------------------------------------------------------------------------------------------------
         ax1
         AS
        ( SELECT Apx.*, ISNULL(UpdatedOn,CreatedOn) LastUpdated
                  ,CASE WHEN StopDate IS NOT NULL THEN 3        
                        WHEN PauseDate IS NOT NULL THEN 6
                        WHEN CompletionDate IS NOT NULL THEN 2 
                        ELSE 1 END CompletionStatus 
                  ,CASE WHEN TrainingCourseVersionConfirmed = 1 THEN TrainingCourseVersion ELSE null END Version
				  ,lv1.Title StandardName
                  ,CASE WHEN EndDate > GETDATE() THEN EOMONTH(EndDate) ELSE EOMONTH(DATEADD(month, lv1.Duration, StartDate)) END EstimatedEndDate
            FROM ApprovalsExtract Apx JOIN LatestVersions lv1 on lv1.LarsCode = apx.TrainingCode
        )
        ----------------------------------------------------------------------------------------------------------------------
        ----------------------------------------------------------------------------------------------------------------------
        INSERT INTO Learner (Id, Uln, GivenNames, FamilyName, UkPrn, StdCode, LearnStartDate, EpaOrgId, FundingModel, ApprenticeshipId, 
        Source, LearnRefNumber, CompletionStatus, PlannedEndDate, DelLocPostCode, LearnActEndDate, WithdrawReason, 
        Outcome, AchDate, OutGrade, Version, CourseOption, StandardUId, StandardReference, StandardName, LastUpdated, EstimatedEndDate )

        ----------------------------------------------------------------------------------------------------------------------
        -- using Approvals Extract as master, except for some key fields that can be updated via ILR, and full Start&End dates 
        ----------------------------------------------------------------------------------------------------------------------
        SELECT  il1.Id, 
                il1.Uln, 
                CASE WHEN ax1.LastUpdated > il1.LastUpdated THEN ax1.FirstName ELSE il1.GivenNames END GivenNames, 
                CASE WHEN ax1.LastUpdated > il1.LastUpdated THEN ax1.LastName ELSE il1.FamilyName END FamilyName, 
                ax1.UkPrn, 
                ax1.TrainingCode StdCode, 
                il1.LearnStartDate, 
                il1.EpaOrgId,     
                il1.FundingModel,
                ax1.ApprenticeshipId,
                'Approvals+' Source,
                ax1.LearnRefNumber,
                CASE WHEN ax1.LastUpdated > il1.LastUpdated THEN ax1.CompletionStatus ELSE il1.CompletionStatus END CompletionStatus,
                il1.PlannedEndDate,
                il1.DelLocPostCode,
                il1.LearnActEndDate,
                il1.WithdrawReason,
                il1.Outcome,
                il1.AchDate,
                il1.OutGrade,    
                ISNULL(ax1.Version,il1.Version) Version,
                ax1.TrainingCourseOption CourseOption,
                ISNULL(ax1.StandardUId,il1.StandardUId) StandardUid,
                ISNULL(ax1.StandardReference,il1.StandardReference) StandardReference,
				ax1.StandardName,
                CASE WHEN ax1.LastUpdated > il1.LastUpdated THEN ax1.LastUpdated ELSE il1.LastUpdated END LastUpdated, 
                il1.EstimatedEndDate
          FROM ax1 
          JOIN il1 ON ax1.ULN = il1.ULN  AND il1.StdCode = ax1.TrainingCode    
          
        ----------------------------------------------------------------------------------------------------------------------
        -- just Apporovals    
        ----------------------------------------------------------------------------------------------------------------------
        UNION 
        SELECT  NEWID() Id, 
                ax1.Uln, 
                ax1.FirstName GivenNames, 
                ax1.LastName FamilyName, 
                ax1.UkPrn, 
                ax1.TrainingCode StdCode, 
                ax1.StartDate LearnStartDate, 
                null EpaOrgId,     
                36 FundingModel,
                ax1.ApprenticeshipId,
                'Approvals' Source,
                ax1.LearnRefNumber,
                ax1.CompletionStatus,
                ax1.EndDate PlannedEndDate,
                null DelLocPostCode,
                null LearnActEndDate,
                null WithdrawReason,
                null Outcome,
                null AchDate,
                null OutGrade,    
                ax1.Version,
                ax1.TrainingCourseOption CourseOption,
                ax1.StandardUId,
                ax1.StandardReference, 
				ax1.StandardName,
                ax1.LastUpdated,
                ax1.EstimatedEndDate
          FROM ax1 
          LEFT JOIN il1 ON ax1.ULN = il1.ULN  AND il1.StdCode = ax1.TrainingCode
          WHERE il1.ULN IS NULL
          
        ----------------------------------------------------------------------------------------------------------------------
        -- just ILrs (trimmed Expired and Lapsed ILR records!)  
        ----------------------------------------------------------------------------------------------------------------------
        UNION 
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
                null CourseOption,
                il1.StandardUId,
                il1.StandardReference,
				il1.StandardName,
                il1.LastUpdated,
                il1.EstimatedEndDate
          FROM il1 
          LEFT JOIN ax1 ON ax1.ULN = il1.ULN  AND il1.StdCode = ax1.TrainingCode
          WHERE ax1.ULN IS NULL
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
