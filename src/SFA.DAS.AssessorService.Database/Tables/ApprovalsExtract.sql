CREATE TABLE [dbo].[ApprovalsExtract]
(
    [ApprenticeshipId] BIGINT NOT NULL PRIMARY KEY, 
    [FirstName] NVARCHAR(100) NULL, 
    [LastName] NVARCHAR(100) NULL, 
    [ULN] BIGINT NULL, 
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
    [PaymentStatus] SMALLINT NULL
    
)
GO

CREATE INDEX [IX_ApprovalsExtract_Uln_StandardCode] ON [ApprovalsExtract] ([Uln], [TrainingCode]) 
GO

CREATE INDEX [IX_ApprovalsExtract_StandardCode_ULN] ON [ApprovalsExtract] ([TrainingCode], [Uln])
INCLUDE ([StartDate],[CreatedOn],[UpdatedOn],[CompletionDate],[EndDate],[StopDate],[PauseDate],[PaymentStatus])
GO
