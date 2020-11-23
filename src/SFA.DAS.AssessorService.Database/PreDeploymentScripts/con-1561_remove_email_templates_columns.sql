/*
	This pre-deployment script will prepare to remove redundant columns from the [EmailTemplates] table before DAC pac
	deployment by temporarily moving the data to a deployment table to pre-empt a deployment error on data drop
*/
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EmailTemplates' AND COLUMN_NAME IN ('Recipients', 'CreatedAt', 'DeletedAt', 'UpdatedAt', 'RecipientTemplate'))
BEGIN
	IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_EmailTemplates_EmailTemplatesRecipients')
	BEGIN
		ALTER TABLE [dbo].[EmailTemplatesRecipients] DROP CONSTRAINT [FK_EmailTemplates_EmailTemplatesRecipients]
	END

	IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EmailTemplates_1561_Deployment')
	BEGIN
		DROP TABLE [EmailTemplates_1561_Deployment]
	END
	
	-- Removing all the data from [EmailTemplate] during deployment so that columns can be dropped by DAC deploy
	SELECT Id, TemplateName, TemplateId INTO EmailTemplates_1561_Deployment	FROM EMailTemplates
	DELETE FROM EMailTemplates
END