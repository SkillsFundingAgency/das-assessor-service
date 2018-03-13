CREATE TABLE [dbo].[Ilrs]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY, 
    [Uln] BIGINT NULL, 
    [GivenNames] NVARCHAR(MAX) NULL, 
    [FamilyName] NVARCHAR(MAX) NULL, 
    [DateOfBirth] DATETIME NULL, 
    [Sex] NVARCHAR(50) NULL, 
    [UkPrn] INT NULL, 
    [StdCode] NVARCHAR(50) NULL, 
    [LearnStartDate] DATETIME NULL, 
    [EpaOrgId] NVARCHAR(50) NULL, 
    [Outcome] NVARCHAR(50) NULL, 
    [AchDate] DATETIME NULL, 
    [OutGrade] NVARCHAR(50) NULL
)
