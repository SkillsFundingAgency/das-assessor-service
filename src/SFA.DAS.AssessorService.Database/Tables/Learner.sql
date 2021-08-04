CREATE TABLE [dbo].[Learner]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Uln] BIGINT NULL, 
    [GivenNames] NVARCHAR(250) NULL, 
    [FamilyName] NVARCHAR(250) NULL, 
    [UkPrn] INT NULL, 
    [StdCode] INT NULL, 
    [LearnStartDate] DATETIME2 NULL, 
    [EpaOrgId] NVARCHAR(50) NULL,     
    [FundingModel] INT NULL,
    [ApprenticeshipId] BIGINT NULL,
    [Source] NVARCHAR(10) NULL, 
    [LearnRefNumber] NVARCHAR(12) NULL,
    [CompletionStatus] [int] NULL,
    [PlannedEndDate] DATETIME2 NULL,
    [DelLocPostCode] [nvarchar](50) NULL,
    [LearnActEndDate] DATETIME2 NULL,
    [WithdrawReason] [int] NULL,
    [Outcome] [int] NULL,
    [AchDate] [datetime] NULL,
    [OutGrade] [nvarchar](50) NULL,    
    [Version] NVARCHAR(10) NULL,
    [CourseOption] NVARCHAR(126) NULL,
    [StandardUId] NVARCHAR(20) NULL,
    [StandardReference] NVARCHAR(10) NULL,
    [StandardName] NVARCHAR(1000) NULL,
    [LastUpdated] DATE NULL,
    [EstimatedEndDate] DATE NULL
    
    
)
GO

CREATE UNIQUE INDEX [IXU_Learner_Uln_StdCode] ON [Learner] ([Uln], [StdCode]) INCLUDE ([FamilyName])
GO

CREATE NONCLUSTERED INDEX [IX_Learner_EpaOrgId_StdCode_CompletionStatus] ON [Learner] ([EpaOrgId], [StdCode], [CompletionStatus]) INCLUDE ([LearnStartDate], [PlannedEndDate], [Uln])
GO

CREATE NONCLUSTERED INDEX [IX_Learner_EpaOrgId_StdCode_Uln] ON [Learner] ([EpaOrgId], [StdCode], [Uln]) INCLUDE ([LearnStartDate], [PlannedEndDate], [CompletionStatus])
GO

CREATE NONCLUSTERED INDEX [IX_Learner_CompletionStatus_StdCode] ON [Learner] ([CompletionStatus], [StdCode]) INCLUDE ([DelLocPostCode], [LearnStartDate], [PlannedEndDate], [Uln])
GO
