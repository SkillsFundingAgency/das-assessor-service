CREATE TABLE [dbo].[ApprovalsExtract_Staging]
(
    [ApprenticeshipId] BIGINT,
    [FirstName] NVARCHAR(100) NULL, 
    [LastName] NVARCHAR(100) NULL, 
    [ULN] BIGINT NULL, 
    [TrainingCode] INT NULL, 
    [TrainingCourseVersion] NVARCHAR(10) NULL,
    [TrainingCourseVersionConfirmed] BIT NULL,
    [TrainingCourseOption] NVARCHAR(126) NULL,
    [StandardUId] NVARCHAR(20) NULL,
    [StartDate] DATETIME NULL, 
    [EndDate] DATETIME NULL, 
    [CreatedOn] DATETIME NULL, 
    [UpdatedOn] DATETIME NULL, 
    [StopDate] DATE NULL, 
    [PauseDate] DATE NULL, 
    [CompletionDate] DATE NULL,
    [UKPRN] INT NULL,
    [LearnRefNumber] NVARCHAR(50) NULL,
    [PaymentStatus] SMALLINT NULL,
    [EmployerAccountId] BIGINT NULL, 
    [EmployerName] NVARCHAR(100) NULL
)
GO
