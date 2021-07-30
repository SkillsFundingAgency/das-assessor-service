CREATE TABLE [dbo].[Ilrs]
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

CREATE UNIQUE INDEX [IXU_Ilrs_Uln_StdCode] ON [Ilrs] ([Uln], [StdCode]) INCLUDE ([FamilyName])
GO

CREATE NONCLUSTERED INDEX [IX_Ilrs_EpaOrgId_StdCode_CompletionStatus] ON [Ilrs] ([EpaOrgId], [StdCode], [CompletionStatus]) INCLUDE ([LearnStartDate], [PlannedEndDate], [Uln])
GO

CREATE NONCLUSTERED INDEX [IX_Ilrs_EpaOrgId_StdCode_Uln] ON [Ilrs] ([EpaOrgId], [StdCode], [Uln]) INCLUDE ([LearnStartDate], [PlannedEndDate], [CompletionStatus])
GO

CREATE NONCLUSTERED INDEX [IX_Ilrs_CompletionStatus_StdCode] ON [Ilrs] ([CompletionStatus], [StdCode]) INCLUDE ([DelLocPostCode], [LearnStartDate], [PlannedEndDate], [Uln])
GO
