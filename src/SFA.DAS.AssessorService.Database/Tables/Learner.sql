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
    [AchDate] DATETIME NULL,
    [OutGrade] [nvarchar](50) NULL,    
    [Version] NVARCHAR(10) NULL,
    [VersionConfirmed] BIT NOT NULL DEFAULT 0,
    [CourseOption] NVARCHAR(126) NULL,
    [StandardUId] NVARCHAR(20) NULL,
    [StandardReference] NVARCHAR(10) NULL,
    [StandardName] NVARCHAR(1000) NULL,
    [LastUpdated] DATE NULL,
    [EstimatedEndDate] DATE NULL,
    [ApprovalsStopDate] DATE NULL,
    [ApprovalsPauseDate] DATE NULL,
    [ApprovalsCompletionDate] DATE NULL,
    [ApprovalsPaymentStatus] SMALLINT NULL,
    [LatestIlrs] DATETIME NULL,
    [LatestApprovals] DATETIME NULL
    
)
GO

CREATE UNIQUE INDEX [IXU_Learner_Uln_StdCode] ON [Learner] ([Uln], [StdCode]) INCLUDE ([FamilyName])
GO

CREATE NONCLUSTERED INDEX [IX_Learner_EpaOrgId_StdCode_Uln_CompletionStatus] ON [Learner] ( [StdCode], [Uln], [CompletionStatus], [EpaOrgId]) INCLUDE ([DelLocPostCode], [LearnStartDate], [PlannedEndDate])
GO

CREATE NONCLUSTERED INDEX [IX_Learner_LatestIlrs] ON [Learner] ([LatestIlrs], [Uln], [StdCode]) 
GO

CREATE NONCLUSTERED INDEX [IX_Learner_LatestApprovals] ON [Learner] ([LatestApprovals], [Uln], [StdCode]) 
GO

CREATE NONCLUSTERED INDEX [IX_Learner_Source] ON [Learner] ([Source], [EstimatedEndDate], [CompletionStatus], [LastUpdated]) 
GO
