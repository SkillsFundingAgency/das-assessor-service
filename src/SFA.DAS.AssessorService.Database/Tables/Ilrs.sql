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
    [PlannedEndDate] DATETIME2 NULL
)

GO

CREATE UNIQUE INDEX [IXU_Ilrs_Uln_StdCode] ON [Ilrs] ([Uln], [StdCode]) INCLUDE ([FamilyName])
GO

CREATE NONCLUSTERED INDEX [IX_Ilrs_EpaOrgId_StdCode_CompletionStatus] ON [Ilrs] ([EpaOrgId], [StdCode], [CompletionStatus]) INCLUDE ([LearnStartDate], [PlannedEndDate], [Uln])
GO

