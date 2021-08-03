CREATE TABLE [dbo].[Learner]
(
    [ApprenticeshipId] BIGINT NOT NULL PRIMARY KEY, 
    [FirstName] NVARCHAR(100) NULL, 
    [LastName] NVARCHAR(100) NULL, 
    [ULN] NVARCHAR(50) NULL, 
    [StandardCode] INT NULL, 
    [TrainingCourseVersion] NVARCHAR(10) NULL,
    [TrainingCourseVersionConfirmed] BIT NOT NULL DEFAULT 0,
    [TrainingCourseOption] NVARCHAR(126) NULL,
    [StandardUId] NVARCHAR(20) NULL,
    [StartDate] DATETIME NULL, 
    [EndDate] DATETIME NULL, 
    [CreatedOn] DATETIME NULL, 
    [UpdatedOn] DATETIME NULL, 
    [AgreedOn] DATETIME NULL, 
    [StopDate] DATE NULL, 
    [PauseDate] DATE NULL, 
    [CompletionDate] DATE NULL,
    [StandardReference] NVARCHAR(10) NULL,
    [UKPRN] INT NULL,
    [LearnRefNumber] NVARCHAR(50) NULL
    
)
GO

CREATE UNIQUE INDEX [IXU_Learner_Uln_StandardCode] ON [Learner] ([Uln], [StandardCode]) INCLUDE ([LastName])
GO
