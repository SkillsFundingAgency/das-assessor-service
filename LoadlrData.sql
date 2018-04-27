-- Script to load legacy Learner Data into the Assessor Service Certificates table from 
-- a CSV file held in Blob Storage.

-- Process --

/* 
Extract Learner data to CSv file
e.g.
SELECT 
   l.ULN, 
   l.LearnRefNumber,
   l.GivenNames, 
   l.FamilyName, 
   l.UKPRN, 
   ld.StdCode, 
   ld.LearnStartDate, 
   ld.EPAOrgID, 
   ld.FundModel,
   s.commitmentidMAX AS ApprenticeshipId
FROM [DS_SILR1718_Collection].[Valid].[Learner] l
LEFT JOIN [Valid].[LearningDelivery] ld on ld.UKPRN = l.UKPRN AND ld.LearnRefNumber = l.LearnRefNumber
LEFT JOIN (SELECT MAX(commitmentid) AS commitmentidMAX, ukprn, LearnRefNumber
           FROM [DAS_PeriodEnd].[DataLock].[PriceEpisodeMatch]
           GROUP BY Ukprn, LearnRefNumber) s on s.Ukprn=l.UKPRN AND s.LearnRefNumber=l.LearnRefNumber
WHERE ld.StdCode is not null
AND AimType = 1


*/

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
	[LearnRefNumber] [bigint] NULL,
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

SET IDENTITY_INSERT Certificates ON

INSERT INTO ilrs (CreatedAt, ULN, LearnRefNumber, GivenNames, FamilyName, UKPRN, StdCode, LearnStartDate, EPAOrgID, FundModel, ApprenticeshipId)
SELECT 
	GETDATE() AS CreatedAt, 
	ULN, LearnRefNumber, GivenNames, FamilyName, UKPRN, StdCode, LearnStartDate, EPAOrgID, FundModel, ApprenticeshipId
FROM LearnerImport 

SET IDENTITY_INSERT Certificates OFF

DROP FUNCTION GetCertificateDataJson
DROP TABLE CertificateImport

DROP EXTERNAL DATA SOURCE BlobStorage
DROP DATABASE SCOPED  CREDENTIAL BlobCredential 
DROP MASTER KEY
