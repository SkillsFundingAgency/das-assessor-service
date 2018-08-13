/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/



IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'TmpContacts'))
BEGIN
    DROP TABLE TmpContacts
END

--IF (EXISTS (SELECT * 
--                 FROM INFORMATION_SCHEMA.TABLES 
--                 WHERE TABLE_SCHEMA = 'dbo' 
--                 AND  TABLE_NAME = 'TmpOrganisations'))
--BEGIN
--    DROP TABLE TmpOrganisations
--END

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'OrganisationTypeId' AND Object_ID = Object_ID(N'dbo.organisations'))
BEGIN
	ALTER TABLE organisations ADD [OrganisationTypeId] [int] NULL, OrganisationData [nvarchar](max) NULL;
END

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PhoneNumber' AND Object_ID = Object_ID(N'dbo.contacts'))
BEGIN
	ALTER TABLE contacts ADD [PhoneNumber] [NVARCHAR] (50) NULL;
END


--SELECT * INTO TmpContacts FROM Contacts
--SELECT * INTO TmpOrganisations FROM Organisations

--DELETE Contacts
--DELETE Organisations