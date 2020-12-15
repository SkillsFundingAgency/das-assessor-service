/*
    Insert or Update each of the [EmailTemplates] look up default values.

    NOTES:

    1) This script uses a temporary table, insert or update the values in the temporary table to apply changes; removed values will
    not take affect (by design); values which are removed should also be written into the EmailTemplatesDelete.sql script to remove
    manually any dependencies, but they must also be removed from the temporary table.
*/
BEGIN TRANSACTION

CREATE TABLE #EMailTemplates(
    [Id] [uniqueidentifier] NOT NULL,
    [TemplateName] [nvarchar](100) NOT NULL,
    [TemplateId] [nvarchar](100) NOT NULL
) 
GO

INSERT INTO #EmailTemplates VALUES('075C643E-BEEF-4BB3-A279-3FB24C144755', 'EPAOPermissionsAmended', 'c1ba00d9-81b6-46d8-9b70-3d89d51aa9c1')
INSERT INTO #EmailTemplates VALUES('EB20EE3C-516E-4E44-97EA-3FD8F70039EF', 'ApplyEPAOResponse', '84174eab-f3c1-4274-8670-2fb5b21cbd77')
INSERT INTO #EmailTemplates VALUES('BCA6B89F-6D77-47C7-87E9-439628ADA40A', 'ApplyEPAOUpdate', 'ffe63c0d-b2b0-461f-b99a-73105d7d5fa3')
INSERT INTO #EmailTemplates VALUES('B66DFD61-5CC3-4A0B-83A2-84F63F3E3371', 'ApplyEPAOInitialSubmission', '68410850-909b-4669-a60a-f60e4b1cb89f')
INSERT INTO #EmailTemplates VALUES('185F9EBD-30B0-4F9D-92A1-851101BF10EA', 'EPAOUserApproveReject', 'e7dc7016-9c88-4e25-9496-cb135001f413')
INSERT INTO #EmailTemplates VALUES('BEA68813-7C76-4D1A-A9A7-AF7942AE8C5E', 'EPAOPrimaryContactAmended', 'f87cb8e2-d544-4edd-8dd6-fa1aeeba584b')
INSERT INTO #EmailTemplates VALUES('DCC27F50-DDD7-4FEA-A60A-C440243B6F22', 'EPAOLoginAccountCreated', '1843d03d-898c-45e5-88d5-8fed1e78cc3b')
INSERT INTO #EmailTemplates VALUES('7142241A-A744-4023-8DB8-E667F6B3F150', 'EPAOUserApproveRequest', 'f7ca95a9-54fb-4f5f-8a88-840445f98c8b')
INSERT INTO #EmailTemplates VALUES('A701F4A4-2672-4DA9-8005-E6EEF1025963', 'ApplyEPAOStandardSubmission', 'e0a52c44-10be-4164-9543-3c312769c4e3')
INSERT INTO #EmailTemplates VALUES('A701F4A4-2672-4DA9-8005-E6EEF10455D0', 'ApplyEPAOAlertSubmission', 'a56c47c8-6310-4f5c-a3f6-9e996c375557')
INSERT INTO #EmailTemplates VALUES('F072BF45-8F43-44C7-9117-E8116A5EF388', 'EPAOOrganisationDetailsAmended', 'd05b7fcd-6aca-4726-8d10-fa36b4172578')
INSERT INTO #EmailTemplates VALUES('DA7603D3-87B1-4040-B272-EDE806538BE8', 'EPAOPermissionsRequested', 'addf58d9-9e20-46fe-b952-7fc62a47b7f7')
INSERT INTO #EmailTemplates VALUES('CA7C8BE2-A6F1-479B-A77D-F0614F7E8924', 'PrintAssessorCoverLetters', '5b171b91-d406-402a-a651-081cce820acb')
INSERT INTO #EmailTemplates VALUES('F5A787F4-0276-4C23-A125-F6D4C1AE0650', 'EPAOUserApproveConfirm', '68506adb-7e17-45c9-ad54-45ef9a2cad15')
INSERT INTO #EmailTemplates VALUES('76DE4F5E-0B82-4729-AD80-DE0C0F54FEEF', 'PrintSasToken', '83b79354-4d2b-476d-b02d-cdb9c016911e')
INSERT INTO #EmailTemplates VALUES('01A8A975-3E19-4B13-A45E-8BA4868957BA', 'WithdrawalEPAOSubmission', 'c9a88290-d4bb-4665-b7cb-525e7b2450d3')

MERGE [EmailTemplates] [Target] USING #EmailTemplates [Source]
ON ([Source].[Id] = [Target].[Id])
WHEN MATCHED
    THEN UPDATE SET 
        [Target].[TemplateName] = [Source].[TemplateName],
        [Target].[TemplateId] = [Source].[TemplateId]

WHEN NOT MATCHED BY TARGET 
    THEN INSERT ([Id], [TemplateName], [TemplateId])
         VALUES ([Source].[Id], [Source].[TemplateName], [Source].[TemplateId]);

COMMIT TRANSACTION

