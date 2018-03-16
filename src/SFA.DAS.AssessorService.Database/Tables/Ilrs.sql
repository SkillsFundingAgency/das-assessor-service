CREATE TABLE [dbo].[Ilrs]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY, 
    [Uln] BIGINT NULL, 
    [GivenNames] NVARCHAR(MAX) NULL, 
    [FamilyName] NVARCHAR(MAX) NULL, 
    [DateOfBirth] DATETIME2 NULL, 
    [Sex] NVARCHAR(50) NULL, 
    [UkPrn] INT NULL, 
    [StdCode] NVARCHAR(50) NULL, 
    [LearnStartDate] DATETIME2 NULL, 
    [EpaOrgId] NVARCHAR(50) NULL, 
    [Outcome] NVARCHAR(50) NULL, 
    [AchDate] DATETIME2 NULL, 
    [OutGrade] NVARCHAR(50) NULL
)
