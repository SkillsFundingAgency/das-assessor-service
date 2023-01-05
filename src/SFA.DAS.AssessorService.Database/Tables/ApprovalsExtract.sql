CREATE TABLE [dbo].[ApprovalsExtract]
(
    [ApprenticeshipId] BIGINT NOT NULL PRIMARY KEY, 
    [FirstName] NVARCHAR(100) NULL, 
    [LastName] NVARCHAR(100) NULL, 
    [Uln] BIGINT NULL, 
    [TrainingCode] INT NULL, 
    [TrainingCourseVersion] NVARCHAR(10) NULL,
    [TrainingCourseVersionConfirmed] BIT NOT NULL DEFAULT 0,
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
    [LastUpdated] AS ISNULL([UpdatedOn],[CreatedOn]),
    [EmployerAccountId] BIGINT NULL, 
    [EmployerName] NVARCHAR(100) NULL
)
GO

CREATE INDEX [IX_ApprovalsExtract_TrainingCode_ULN] ON [ApprovalsExtract] ([TrainingCode], [Uln], [StartDate], [PaymentStatus] ,[StopDate]) 
INCLUDE ([ApprenticeshipId] ,[FirstName] ,[LastName] ,[TrainingCourseVersion] ,[TrainingCourseVersionConfirmed] ,[TrainingCourseOption],
         [StandardUId] ,[EndDate] ,[CreatedOn] ,[UpdatedOn] ,[PauseDate] ,[CompletionDate] ,[UKPRN] ,[LearnRefNumber] ,[LastUpdated], [EmployerAccountId], [EmployerName])
      
GO

CREATE NONCLUSTERED INDEX [IX_ApprovalsExtract_LastUpdated] ON [ApprovalsExtract] ([LastUpdated]) INCLUDE ([TrainingCode], [Uln] )
GO

CREATE NONCLUSTERED INDEX [IX_ApprovalsExtract_Ukprn] ON [ApprovalsExtract] ([Ukprn])
GO
