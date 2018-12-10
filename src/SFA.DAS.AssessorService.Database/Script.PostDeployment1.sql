/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- Amend Organisations Type to have description

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'OrganisationType'
    AND column_name = 'TypeDescription'
)
BEGIN
    ALTER TABLE [OrganisationType] ADD [TypeDescription] nvarchar(500);
END
GO

UPDATE [OrganisationType] SET [Type] =  'Awarding Organisations', [TypeDescription] = 'Awarding organisations' WHERE id = 1;
UPDATE [OrganisationType] SET [Type] =  'Assessment Organisations', [TypeDescription] = 'Assessment organisations' WHERE id = 2;
UPDATE [OrganisationType] SET [Type] =  'Trade Body', [TypeDescription] = 'Trade body' WHERE id = 3;
UPDATE [OrganisationType] SET [Type] =  'Professional Body', [TypeDescription] = 'Professional body - approved by the relevant council' WHERE id = 4;
UPDATE [OrganisationType] SET [Type] =  'HEI', [TypeDescription] = 'HEI monitored and supported by OfS' WHERE id = 5;
UPDATE [OrganisationType] SET [Type] =  'NSA or SSC', [TypeDescription] = 'National skills academy or sector skills council' WHERE id = 6;
UPDATE [OrganisationType] SET [Type] =  'Training Provider', [TypeDescription] = 'Training Provider - including HEI not in England' WHERE id = 7;
UPDATE [OrganisationType] SET [Status] =  'Deleted' WHERE id = 8;
UPDATE [OrganisationType] SET [Type] =  'Public Sector', [TypeDescription] = 'Incorporated as a public sector body' WHERE id = 9;

-- 'College'
IF NOT EXISTS(SELECT * FROM OrganisationType WHERE id = 10)
BEGIN
	SET identity_insert OrganisationType ON
	INSERT INTO [OrganisationType] (ID,[Type],[Status], [TypeDescription]) VALUES (10,'College','Live','GFE college currently receiving funding from the ESFA, 6th form or FE college');
	SET identity_insert OrganisationType OFF
END
ELSE
BEGIN
	UPDATE [OrganisationType] SET [Type] =  'College', [TypeDescription] = 'GFE college currently receiving funding from the ESFA, 6th form or FE college' WHERE id = 10;
END

IF NOT EXISTS(SELECT * FROM StaffReports)
	BEGIN
		INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Monthly detailed extract', 'StaffReports_DetailedExtract', 1,'ViewOnScreen')
		INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Monthly summary', 'StaffReports_MonthlySummary', 2,'ViewOnScreen')
		INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Weekly summary', 'StaffReports_WeeklySummary', 3,'ViewOnScreen')
		INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Batch', 'StaffReports_ByBatch', 4,'ViewOnScreen')
		INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('EPAO', 'StaffReports_ByEpao', 5,'ViewOnScreen')
		INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('EPAO, standard and grade', 'StaffReports_ByEpaoAndStandardAndGrade', 6,'ViewOnScreen')
		INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Provider', 'StaffReports_ByProvider', 7,'ViewOnScreen')
		INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Provider and grade', 'StaffReports_ByProviderAndGrade', 8,'ViewOnScreen')
		INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Standard', 'StaffReports_ByStandard', 9,'ViewOnScreen')
		INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Certificate count', 'StaffReports_CertificateCount', 10,'ViewOnScreen')
	END

UPDATE StaffReports SET ReportType = 'ViewOnScreen' WHERE ReportType IS NULL

IF NOT EXISTS (SELECT * FROM StaffReports WHERE ReportName = 'EPAO Register')
BEGIN
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('EPAO Register', '', 11, 'Download')
END

UPDATE StaffReports SET ReportDetails ='{"Name":"EPAO Register","Worksheets": [
  {
  "worksheet":"Register - Organisations",
  "order": 1,
  "StoredProcedure":"EPAO_Register_register_organisation"
  },
  {
  "worksheet":"Register - Standards",
  "order": 2,
  "StoredProcedure":"EPAO_Register_register_standards"
  },
  {
  "worksheet":"Register - Delivery areas",
  "order": 3,
  "StoredProcedure":"EPAO_Register_register_delivery_areas"
  },
  {
  "worksheet":"Data Definitions",
  "order": 4,
  "StoredProcedure":"EPAO_Register_Data_Definitions"
  }
  ]}' WHERE ReportName = 'EPAO Register'



IF NOT EXISTS (SELECT * FROM StaffReports WHERE ReportName = 'Register List of Organisations')
BEGIN
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Register List of Organisations', '', 12, 'Download')
END

UPDATE StaffReports SET ReportDetails = '{"Name":"Register List Of Organisations","Worksheets": [
	  {
	  "worksheet":"Register List of Organisations",
	  "order": 1,
	  "StoredProcedure":"EPAO_Register_list_of_organisations"
	  }
	]}'  WHERE ReportName = 'Register List of Organisations'


IF NOT EXISTS (SELECT * FROM StaffReports WHERE ReportName = 'Register List of Standards')
BEGIN
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Register List of Standards', '', 13, 'Download')
END

UPDATE StaffReports SET ReportDetails = '{"Name":"Register List Of Standards","Worksheets": [
	  {
	  "worksheet":"Register List of Standards",
	  "order": 1,
	  "StoredProcedure":"EPAO_Register_list_of_standards"
	  }
	]}'  WHERE ReportName = 'Register List of Standards'


-- -'Academy or Free School'
IF NOT EXISTS(SELECT * FROM OrganisationType WHERE id = 11)
BEGIN
	SET identity_insert OrganisationType ON
	INSERT INTO [OrganisationType] (ID,[Type],[Status], [TypeDescription]) VALUES (11,'Academy or Free School','Live','Academy or Free school registered with the ESFA');
	SET identity_insert OrganisationType OFF
END
ELSE
BEGIN
	UPDATE [OrganisationType] SET [Type] =  'Academy or Free School', [TypeDescription] = 'Academy or Free school registered with the ESFA' WHERE id = 11;
END


UPDATE [Organisations] SET [OrganisationTypeId] = 3 WHERE [EndPointAssessorOrganisationId] = 'EPA0027';
UPDATE [Organisations] SET [OrganisationTypeId] = 9 WHERE [EndPointAssessorOrganisationId] = 'EPA0035';
UPDATE [Organisations] SET [OrganisationTypeId] = 4 WHERE [EndPointAssessorOrganisationId] = 'EPA0049';
UPDATE [Organisations] SET [OrganisationTypeId] = 6 WHERE [EndPointAssessorOrganisationId] = 'EPA0051';
UPDATE [Organisations] SET [OrganisationTypeId] = 9 WHERE [EndPointAssessorOrganisationId] = 'EPA0085';
UPDATE [Organisations] SET [OrganisationTypeId] = 6 WHERE [EndPointAssessorOrganisationId] = 'EPA0104';
UPDATE [Organisations] SET [OrganisationTypeId] = 9 WHERE [EndPointAssessorOrganisationId] = 'EPA0141';
UPDATE [Organisations] SET [OrganisationTypeId] = 10 WHERE [EndPointAssessorOrganisationId] = 'EPA0159';
UPDATE [Organisations] SET [OrganisationTypeId] = 7 WHERE [EndPointAssessorOrganisationId] = 'EPA0173';


UPDATE CERTIFICATES
set IsPrivatelyFunded = 0
WHERE IsPrivatelyFunded IS NULL 


--- FIXES FOR BAD DATA IN SPREADSHEET IMPORT

if not exists(select * from OrganisationStandardDeliveryArea where organisationStandardId in (select id from organisationStandard where EndPointAssessorOrganisationId='EPA0048' and StandardCode=320))
BEGIN
insert into OrganisationStandardDeliveryArea (OrganisationStandardId, DeliveryAreaId, Status)
		select 
			(select id from organisationStandard where EndPointAssessorOrganisationId='EPA0048' and StandardCode=320) as osid, id, 'Live' from deliveryArea
END


if not exists(select * from OrganisationStandardDeliveryArea where organisationStandardId in (select id from organisationStandard where EndPointAssessorOrganisationId='EPA0057' and StandardCode=318))
BEGIN
insert into OrganisationStandardDeliveryArea (OrganisationStandardId, DeliveryAreaId, Status)
		select 
			(select id from organisationStandard where EndPointAssessorOrganisationId='EPA0057' and StandardCode=318) as osid, id, 'Live' from deliveryArea
END

----- Adding 