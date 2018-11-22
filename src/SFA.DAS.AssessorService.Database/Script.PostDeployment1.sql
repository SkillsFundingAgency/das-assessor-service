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

UPDATE [OrganisationType] SET [Type] =  'Awarding Organisations', [TypeDescription] = 'Awarding Organisations' WHERE id = 1;
UPDATE [OrganisationType] SET [Type] =  'Assessment Organisations', [TypeDescription] = 'Assessment Organisations' WHERE id = 2;
UPDATE [OrganisationType] SET [Type] =  'Trade Body', [TypeDescription] = 'Trade Body' WHERE id = 3;
UPDATE [OrganisationType] SET [Type] =  'Professional Body', [TypeDescription] = 'Professional Body - approved by the relevant council' WHERE id = 4;
UPDATE [OrganisationType] SET [Type] =  'HEI', [TypeDescription] = 'HEI monitored and supported by OfS' WHERE id = 5;
UPDATE [OrganisationType] SET [Type] =  'NSA or SSC', [TypeDescription] = 'National Skills Academy / Sector Skills Council' WHERE id = 6;
UPDATE [OrganisationType] SET [Type] =  'Training Provider', [TypeDescription] = 'Training Provider - including HEI not in England' WHERE id = 7;
UPDATE [OrganisationType] SET [Status] =  'Deleted' WHERE id = 8;
UPDATE [OrganisationType] SET [Type] =  'Public Sector', [TypeDescription] = 'Incorporated as Public Sector Body, Local authority including LEA schools, Central Government Department / Executive Agency / Non-departmental public body, NHS Trust / Fire Authority, Police Constabulary or Police Crime Commissioner' WHERE id = 9;

-- 'College'
IF NOT EXISTS(SELECT * FROM OrganisationType WHERE id = 10)
BEGIN
	SET identity_insert OrganisationType ON
	INSERT INTO [OrganisationType] (ID,[Type],[Status], [TypeDescription]) VALUES (10,'College','Live','GFE College currently receiving funding from the ESFA, 6th form / FE college');
	SET identity_insert OrganisationType OFF
END
ELSE
BEGIN
	UPDATE [OrganisationType] SET [Type] =  'College', [TypeDescription] = 'GFE College currently receiving funding from the ESFA, 6th form / FE college' WHERE id = 10;
END

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