-- This script will add data an additional email template; this will require a manual data fix script 
-- per environment in which assoicated [EmailTemplateRecipients] rows are required

IF NOT EXISTS (SELECT 1 FROM [EmailTemplates] WHERE TemplateName = 'PrintSasToken')
BEGIN
	INSERT INTO [dbo].[EMailTemplates] ([Id], [TemplateName], [TemplateId], [Recipients], [CreatedAt], [DeletedAt], [UpdatedAt], [RecipientTemplate])
	VALUES ('76DE4F5E-0B82-4729-AD80-DE0C0F54FEEF', 'PrintSasToken', '83b79354-4d2b-476d-b02d-cdb9c016911e', NULL, GETUTCDATE(), NULL, NULL, NULL)
END
