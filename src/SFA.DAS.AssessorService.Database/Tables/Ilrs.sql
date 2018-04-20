CREATE TABLE [dbo].[Ilrs]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY, 
    [Uln] BIGINT NULL, 
    [GivenNames] NVARCHAR(MAX) NULL, 
    [FamilyName] NVARCHAR(MAX) NULL, 
    [UkPrn] INT NULL, 
    [StdCode] NVARCHAR(50) NULL, 
    [LearnStartDate] DATETIME2 NULL, 
    [EpaOrgId] NVARCHAR(50) NULL,     
	[FundingModel] INT NULL,
    [ApprenticeshipId] BIGINT NULL,
    [EmployerAccountId] BIGINT NULL,
    [Source] NVARCHAR(10) NULL
)
