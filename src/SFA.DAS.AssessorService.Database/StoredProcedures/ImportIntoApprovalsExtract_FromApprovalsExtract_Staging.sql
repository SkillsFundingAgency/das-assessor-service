CREATE PROCEDURE [dbo].[ImportIntoApprovalsExtract_FromApprovalsExtract_Staging]
AS

	WITH ApprenticeshipImportCount As(
	SELECT ApprenticeshipId, Count(*) Total FROM [dbo].ApprovalsExtract_Staging
	GROUP BY ApprenticeshipId),

	DuplicateStagingRecords as (
	SELECT ROW_NUMBER() OVER (PARTITION BY ApprenticeshipId ORDER BY CreatedOn) as RowNumber, ApprenticeshipId FROM [dbo].ApprovalsExtract_Staging
	WHERE ApprenticeshipId in (SELECT ApprenticeshipId from ApprenticeshipImportCount WHERE Total > 1))
	
	-- For some reason the import with batches returns duplicate entries on occassion
	-- Potentially if the data in commitments shifts under us, therefore before import
	-- take care of duplicates then merge / insert
	DELETE FROM DuplicateStagingRecords where RowNumber != 1
	
	MERGE INTO [dbo].[ApprovalsExtract] AS TARGET
    USING [dbo].[ApprovalsExtract_Staging] AS SOURCE
    ON TARGET.ApprenticeshipId = SOURCE.ApprenticeshipId
    
	WHEN NOT MATCHED BY TARGET
        THEN INSERT (ApprenticeshipId, FirstName, LastName, ULN, TrainingCode, TrainingCourseVersion, TrainingCourseVersionConfirmed, TrainingCourseOption,
		StandardUId, StartDate, EndDate, CreatedOn, UpdatedOn, StopDate, PauseDate, CompletionDate, UKPRN, LearnRefNumber, PaymentStatus, EmployerAccountId, EmployerName)
    VALUES (SOURCE.ApprenticeshipId, SOURCE.FirstName, SOURCE.LastName,SOURCE.ULN, SOURCE.TrainingCode, SOURCE.TrainingCourseVersion, SOURCE.TrainingCourseVersionConfirmed,
			SOURCE.TrainingCourseOption, SOURCE.StandardUId, SOURCE.StartDate, SOURCE.EndDate, SOURCE.CreatedOn, SOURCE.UpdatedOn, SOURCE.StopDate, SOURCE.PauseDate,
			SOURCE.CompletionDate, SOURCE.UKPRN, SOURCE.LearnRefNumber, SOURCE.PaymentStatus, SOURCE.EmployerAccountId, SOURCE.EmployerName)
	WHEN MATCHED THEN UPDATE SET
        TARGET.FirstName		= SOURCE.FirstName,
		TARGET.LastName			= SOURCE.LastName,
		TARGET.Uln				= SOURCE.Uln,
		TARGET.TrainingCode		= SOURCE.TrainingCode,
		TARGET.StandardUId		= SOURCE.StandardUId,
		TARGET.StartDate		= SOURCE.StartDate,
		TARGET.EndDate			= SOURCE.EndDate,
		TARGET.CreatedOn		= SOURCE.CreatedOn,
		TARGET.UpdatedOn		= SOURCE.UpdatedOn,
		TARGET.StopDate			= SOURCE.StopDate,
		TARGET.PauseDate		= SOURCE.PauseDate,
		TARGET.CompletionDate	= SOURCE.CompletionDate,
		TARGET.UKPRN			= SOURCE.UKPRN,
		TARGET.LearnRefNumber	= SOURCE.LearnRefNumber,
		TARGET.PaymentStatus	= SOURCE.PaymentStatus,
		TARGET.TrainingCourseVersion= SOURCE.TrainingCourseVersion,
		TARGET.TrainingCourseVersionConfirmed= SOURCE.TrainingCourseVersionConfirmed,
		TARGET.TrainingCourseOption= SOURCE.TrainingCourseOption,
		TARGET.EmployerAccountId= SOURCE.EmployerAccountId,
		TARGET.EmployerName = SOURCE.EmployerName;

RETURN 0