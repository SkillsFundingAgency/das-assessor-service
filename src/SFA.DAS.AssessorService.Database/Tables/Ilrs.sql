CREATE TABLE [dbo].[Ilrs]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY, 
    [Uln] BIGINT NULL, 
    [GivenNames] NVARCHAR(250) NULL, 
    [FamilyName] NVARCHAR(250) NULL, 
    [UkPrn] INT NULL, 
    [StdCode] NVARCHAR(50) NULL, 
    [LearnStartDate] DATETIME2 NULL, 
    [EpaOrgId] NVARCHAR(50) NULL,     
	[FundingModel] INT NULL,
    [ApprenticeshipId] BIGINT NULL,
    [EmployerAccountId] BIGINT NULL,
    [Source] NVARCHAR(10) NULL, 
    [CreatedAt] DATETIME2 NOT NULL, 
    [UpdatedAt] DATETIME2 NULL,
	[LearnRefNumber] NVARCHAR(12) NULL
)

GO

CREATE INDEX [IX_Ilrs_Uln_FamilyName_StdCode] ON [dbo].[Ilrs] ([Uln], [FamilyName], [StdCode])
