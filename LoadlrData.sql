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
SECRET = 'sv=2017-07-29&ss=bfqt&srt=sco&sp=r&se=2018-04-30T16:25:09Z&st=2018-04-26T08:25:09Z&spr=https&sig=gAxvJPMRYsosb3MN0S4u%2F3UuhZ7nR%2FqK16eYsjkGGdM%3D';

---- Create an external data source with CREDENTIAL option.
CREATE EXTERNAL DATA SOURCE BlobStorage WITH (
    TYPE = BLOB_STORAGE, 
    LOCATION = 'https://esfatempstorage.blob.core.windows.net/import',
    CREDENTIAL = BlobCredential
);

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
	[ApprenticeshipId] [bigint] NULL
)
GO

BULK INSERT LearnerImport 
FROM 'learners.csv'
WITH (DATA_SOURCE = 'BlobStorage', FORMAT = 'CSV', FIRSTROW= 2)

TRUNCATE TABLE Ilrs

INSERT INTO Ilrs (CreatedAt, ULN, LearnRefNumber, GivenNames, FamilyName, UKPRN, StdCode, LearnStartDate, EPAOrgID, FundingModel, ApprenticeshipId, EmployerAccountId)
SELECT 
	GETDATE() AS CreatedAt, 
	ULN, LearnRefNumber, GivenNames, FamilyName, UKPRN, StdCode, LearnStartDate, EPAOrgID, FundingModel, ApprenticeshipId, 0 AS EmployerAccountId
FROM LearnerImport 

DROP TABLE LearnerImport

DROP EXTERNAL DATA SOURCE BlobStorage
DROP DATABASE SCOPED CREDENTIAL BlobCredential 
DROP MASTER KEY
