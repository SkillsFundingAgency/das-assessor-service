-- Script to load legacy Certificate Data into the Assessor Service Certificates table from 
-- a CSV file held in Blob Storage.

-- Process --

-- In Excel:
-- Open certificates xls.
-- Remove Columns: 
--    Provider Name
--    Awarding Org
--    Employer Reference Number
--    Date of Birth
--    Sex
--    Warnings
-- Export to csv named certs.csv

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

CREATE TABLE [dbo].[CertificateImport](
	[Year] [nvarchar](4) NULL,
	[Return] [int] NULL,
	[Source] [nvarchar](10) NULL,
	[ID] [nvarchar](50) NULL,
	[UKPRN] [bigint] NULL,
	[EpaUln] [nvarchar](50) NULL,
	[EmployerName] [nvarchar](200) NULL,
	[EmployerContact] [nvarchar](100) NULL,
	[EmployerAddressLine1] [nvarchar](100) NULL,
	[EmployerAddressLine2] [nvarchar](100) NULL,
	[EmployerAddressLine3] [nvarchar](100) NULL,
	[EmployerAddressTownOrCity] [nvarchar](100) NULL,
	[EmployerPostcode] [nvarchar](50) NULL,
	[Uln] [bigint] NULL,
	[FullName] [nvarchar](200) NULL,
	[StandardCode] [int] NULL,
	[StandardName] [nvarchar](200) NULL,
	[Level] [nvarchar](50) NULL,
	[Option] [nvarchar](50) NULL,
	[PublicationDate] [nvarchar](50) NULL,
	[OverallGrade] [nvarchar](50) NULL,
	[LearningStartDate] [nvarchar](50) NULL,
	[AchievementDate] [nvarchar](50) NULL
)
GO

CREATE FUNCTION GetCertificateDataJson 
(
	-- Add the parameters for the function here
	@CertificateId int
)
RETURNS nvarchar(max)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Json nvarchar(max)

	-- Add the T-SQL statements to compute the return value here
	SELECT @Json = (SELECT TOP (1) 
		i.GivenNames AS LearnerGivenNames,
		i.FamilyName AS LearnerFamilyName,	
		ci.StandardName AS StandardName,
		ci.Level AS StandardLevel,
		CAST(ci.PublicationDate AS datetime2) AS StandardPublicationDate,
		ci.EmployerContact AS ContactName,
		ci.EmployerName AS ContactOrganisation,
		ci.EmployerAddressLine1 AS ContactAddLine1,
		ci.EmployerAddressLine2 AS ContactAddLine2,
		ci.EmployerAddressLine3 AS ContactAddLine3,
		ci.EmployerAddressTownOrCity AS ContactAddLine4,
		ci.EmployerPostcode AS ContactPostCode,
		null AS Registration,
		CAST(ci.LearningStartDate AS datetime2) AS LearningStartDate,
		CAST(ci.AchievementDate AS datetime2) AS AchievementDate,
		ci.[Option] AS CourseOption,
		ci.OverallGrade AS OverallGrade,
		null AS Department
	FROM CertificateImport ci 
	LEFT OUTER JOIN Ilrs i ON i.Uln = ci.Uln
	WHERE ci.Id = @CertificateId FOR JSON PATH, INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER)

	-- Return the result of the function
	RETURN @Json
END
GO

BULK INSERT CertificateImport 
FROM 'certs.csv'
WITH (DATA_SOURCE = 'BlobStorage', FORMAT = 'CSV', FIRSTROW= 2)

SET IDENTITY_INSERT Certificates ON

INSERT INTO Certificates (CreatedAt, ToBePrinted, CertificateReferenceId, CertificateReference, ProviderUkPrn, OrganisationId, Uln, StandardCode, BatchNumber, Status, CreatedBy, CertificateData)
SELECT 
	GETDATE() AS CreatedAt, 
	GETDATE() AS ToBePrinted, 
	ci.ID AS CertificateReferenceId,
	ci.ID AS CertificateReference,
	ci.UkPrn AS ProviderUkPrn,
	o.Id AS OrganisationId,
	ci.Uln AS Uln,
	ci.StandardCode AS StandardCode,
	1 AS BatchNumber,
	'Printed' AS Status,
	'Manual' AS CreatedBy,
	dbo.GetCertificateDataJson(ci.ID) AS CertificateData
FROM CertificateImport ci
INNER JOIN Organisations o ON o.EndPointAssessorOrganisationId = ci.EpaUln
WHERE ci.Source = 'ILR'
AND NOT EXISTS (SELECT null FROM Certificates ce WHERE ce.CertificateReference = ci.ID)

SET IDENTITY_INSERT Certificates OFF

DROP FUNCTION GetCertificateDataJson
DROP TABLE CertificateImport

DROP EXTERNAL DATA SOURCE BlobStorage
DROP DATABASE SCOPED  CREDENTIAL BlobCredential 
DROP MASTER KEY
