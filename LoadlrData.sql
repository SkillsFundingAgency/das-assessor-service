-- Script to load legacy Learner Data into the Assessor Service Ilrs table from 
-- a CSV file held in Blob Storage.

-- Process --


-- Open CSV in notepad++ and replace fake-spaces with real spaces (found in the Standard Name column)
-- Remove blank rows
-- Upload csv to blob storage.

-- Replace Blob Storage Location (including container) & SAS Secret below
-- Run script.
-- With current data, it should finish with:
--(1668 row(s) affected)
--(1170 row(s) affected)

-- Create a database master key if one does not already exist, using your own password. This key is used to encrypt the credential secret in next step.
CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'Baldy N3rd Face';

---- Create a database scoped credential with Azure storage account key as the secret.
CREATE DATABASE SCOPED CREDENTIAL BlobCredential 
WITH IDENTITY = 'SHARED ACCESS SIGNATURE', 
SECRET = 'st=2018-06-29T07%3A46%3A11Z&se=2018-06-30T07%3A46%3A11Z&sp=rwdl&sv=2017-04-17&sr=c&sig=5GtueHBsul4XXDb2EsBWnjaSgWO%2BwsEjzxqMNBhbrRI%3D';

---- Create an external data source with CREDENTIAL option.
CREATE EXTERNAL DATA SOURCE BlobStorage WITH (
    TYPE = BLOB_STORAGE, 
    LOCATION = 'https://esfatempstorage.blob.core.windows.net/import',
    CREDENTIAL = BlobCredential
);

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'LearnerImport'))
BEGIN
    DROP TABLE [LearnerImport]
END

CREATE TABLE [dbo].[LearnerImport](
	[Uln] [bigint] NULL,
	[LearnRefNumber] [nvarchar](12) NULL,
	[GivenNames] [nvarchar](250) NULL,
	[FamilyName] [nvarchar](250) NULL,
	[UkPrn] [int] NULL,
	[StdCode] [int] NULL,
	[LearnStartDate] [nvarchar](50) NULL,
	[EpaOrgId] [nvarchar](50) NULL,
	[FundingModel] [int] NULL,
	[ApprenticeshipId] [bigint] NULL,
	[CompletionStatus] [int] NULL
)
GO

BULK INSERT LearnerImport 
FROM 'learners.csv'
WITH (DATA_SOURCE = 'BlobStorage', FORMAT = 'CSV', FIRSTROW= 2)

INSERT INTO Ilrs (CreatedAt, ULN, LearnRefNumber, GivenNames, FamilyName, UKPRN, StdCode, LearnStartDate, EPAOrgID, FundingModel, ApprenticeshipId, EmployerAccountId)
SELECT 
	GETDATE() AS CreatedAt, 
	li.ULN, li.LearnRefNumber, li.GivenNames, li.FamilyName, li.UKPRN, li.StdCode, li.LearnStartDate, li.EPAOrgID, li.FundingModel, li.ApprenticeshipId, 0 AS EmployerAccountId
	FROM LearnerImport li
	LEFT OUTER JOIN Ilrs i ON i.Uln = li.ULN AND i.LearnRefNumber = li.LearnRefNumber AND i.StdCode = li.StdCode AND i.UkPrn = li.UKPRN AND i.LearnStartDate = li.LearnStartDate
	WHERE i.Uln IS NULL

UPDATE Ilrs SET FamilyName = REPLACE(REPLACE(REPLACE(FamilyName, '`',''''),'’',''''),'–','-'), GivenNames = REPLACE(REPLACE(REPLACE(GivenNames, '`',''''),'’',''''),'–','-')

DROP EXTERNAL DATA SOURCE BlobStorage
DROP DATABASE SCOPED CREDENTIAL BlobCredential 
DROP MASTER KEY
