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
IF NOT EXISTS(SELECT * FROM ScheduleRuns)
BEGIN
	INSERT INTO ScheduleRuns (RunTime, IsComplete, Interval, IsRecurring, ScheduleType) VALUES ('2018-08-17 08:00:00', 0, 10080, 1, 1)
END

IF NOT EXISTS(SELECT * FROM StaffReports)
BEGIN
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Monthly detailed extract', 'StaffReports_DetailedExtract', 1)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Monthly summary', 'StaffReports_MonthlySummary', 2)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Weekly summary', 'StaffReports_WeeklySummary', 3)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Batch', 'StaffReports_ByBatch', 4)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('EPAO', 'StaffReports_ByEpao', 5)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('EPAO, standard and grade', 'StaffReports_ByEpaoAndStandardAndGrade', 6)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Provider', 'StaffReports_ByProvider', 7)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Provider and grade', 'StaffReports_ByProviderAndGrade', 8)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Standard', 'StaffReports_ByStandard', 9)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Certificate count', 'StaffReports_CertificateCount', 10)
END

UPDATE CERTIFICATES
set IsPrivatelyFunded = 0
WHERE IsPrivatelyFunded IS NULL 


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

UPDATE [OrganisationType] SET [Type] =  'AwardingOrganisations', [TypeDescription] = 'Awarding Organisations' WHERE id = 1;
UPDATE [OrganisationType] SET [Type] =  'AssessmentOrganisations', [TypeDescription] = 'Assessment Organisations' WHERE id = 2;
UPDATE [OrganisationType] SET [Type] =  'TradeBody', [TypeDescription] = 'Trade Body' WHERE id = 3;
UPDATE [OrganisationType] SET [Type] =  'ProfessionalBody', [TypeDescription] = 'Professional Body - approved by the relevant council' WHERE id = 4;
UPDATE [OrganisationType] SET [Type] =  'HEI', [TypeDescription] = 'HEI monitored and supported by OfS' WHERE id = 5;
UPDATE [OrganisationType] SET [Type] =  'NSA_SSC', [TypeDescription] = 'National Skills Academy / Sector Skills Council' WHERE id = 6;
UPDATE [OrganisationType] SET [Type] =  'TrainingProvider', [TypeDescription] = 'Training Provider - including HEI not in England' WHERE id = 7;
UPDATE [OrganisationType] SET [Status] =  'Deleted' WHERE id = 8;
UPDATE [OrganisationType] SET [Type] =  'PublicSector', [TypeDescription] = 'Incorporated as Public Sector Body, Local authority including LEA schools, Central Government Department / Executive Agency / Non-departmental public body, NHS Trust / Fire Authority, Police Constabulary or Police Crime Commissioner' WHERE id = 9;

-- 'college','Academy_FreeSchool'

IF NOT EXISTS(SELECT * FROM OrganisationType WHERE ID IN (10,11))
BEGIN
	SET identity_insert OrganisationType ON
	INSERT INTO [OrganisationType] (ID,[Type],[Status], [TypeDescription]) VALUES (10,'College','Live','GFE College currently receiving funding from the ESFA, 6th form / FE college');
	INSERT INTO [OrganisationType] (ID,[Type],[Status], [TypeDescription]) VALUES (11,'Academy_FreeSchool','Live','Academy or Free school registered with the ESFA');
	SET identity_insert OrganisationType OFF
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