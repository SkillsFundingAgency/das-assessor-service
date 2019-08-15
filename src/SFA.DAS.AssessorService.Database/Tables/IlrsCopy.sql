CREATE TABLE [dbo].[IlrsCopy]
(
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY, 
    [Uln] BIGINT NULL, 
    [GivenNames] NVARCHAR(250) NULL, 
    [FamilyName] NVARCHAR(250) NULL, 
    [UkPrn] INT NULL, 
    [StdCode] INT NULL, 
    [LearnStartDate] DATETIME2 NULL, 
    [EpaOrgId] NVARCHAR(50) NULL,     
    [FundingModel] INT NULL,
    [ApprenticeshipId] BIGINT NULL,
    [EmployerAccountId] BIGINT NULL,
    [Source] NVARCHAR(10) NULL, 
    [CreatedAt] DATETIME2 NOT NULL, 
    [UpdatedAt] DATETIME2 NULL,
    [LearnRefNumber] NVARCHAR(12) NULL,
    [CompletionStatus] [int] NULL, 
    [EventId] BIGINT NULL, 
    [PlannedEndDate] DATETIME2 NULL,
    [DelLocPostCode] [nvarchar](50) NULL,
    [LearnActEndDate] DATETIME2 NULL,
    [WithdrawReason] [int] NULL,
    [Outcome] [int] NULL,
    [AchDate] [datetime] NULL,
    [OutGrade] [nvarchar](50) NULL
)

GO
