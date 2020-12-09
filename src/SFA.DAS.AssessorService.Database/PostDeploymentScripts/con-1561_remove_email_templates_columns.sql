/*
	This post-deployment script will restore data into the [EmailTemplates] table after DAC pac
	table update scripts has completed; the data was previously removed to pre-empt a deployment error on data drop
*/
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EmailTemplates_1561_Deployment')
BEGIN
	-- Restoring all the data into [EmailTemplate] after table update deployment stage
	INSERT INTO [EMailTemplates] SELECT Id, TemplateName, TemplateId FROM EmailTemplates_1561_Deployment
	
	-- Dropping this temporary deployment table
	DROP TABLE EmailTemplates_1561_Deployment

	ALTER TABLE [dbo].[EmailTemplatesRecipients]  WITH CHECK ADD  CONSTRAINT [FK_EmailTemplates_EmailTemplatesRecipients] FOREIGN KEY([EmailTemplateId])
	REFERENCES [dbo].[EMailTemplates] ([Id])
END
