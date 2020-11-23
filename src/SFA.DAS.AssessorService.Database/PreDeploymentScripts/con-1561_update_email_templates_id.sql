/*
	This pre-deployment script will update the [EmailTemplateRecipients] and [EmailTemplates] tables to sychronize the
	Id columns with the default lookup data for [EmailTemplate], prior to adding a foreign key to enforce the 
	correct data integrity and the post-deployment script for synchronizing all the columns in [EmailTemplates]
	with the default lookup data
*/
-- these Id's are from the PROD system and are now used as the default lookup data Id's
DECLARE @DefaultEPAOPermissionsAmended UNIQUEIDENTIFIER = '075C643E-BEEF-4BB3-A279-3FB24C144755'
DECLARE @DefaultApplyEPAOResponse UNIQUEIDENTIFIER = 'EB20EE3C-516E-4E44-97EA-3FD8F70039EF'
DECLARE @DefaultApplyEPAOUpdate UNIQUEIDENTIFIER = 'BCA6B89F-6D77-47C7-87E9-439628ADA40A'
DECLARE @DefaultApplyEPAOInitialSubmission UNIQUEIDENTIFIER = 'B66DFD61-5CC3-4A0B-83A2-84F63F3E3371'
DECLARE @DefaultEPAOUserApproveReject UNIQUEIDENTIFIER = '185F9EBD-30B0-4F9D-92A1-851101BF10EA'
DECLARE @DefaultEPAOPrimaryContactAmended UNIQUEIDENTIFIER = 'BEA68813-7C76-4D1A-A9A7-AF7942AE8C5E'
DECLARE @DefaultEPAOLoginAccountCreated UNIQUEIDENTIFIER = 'DCC27F50-DDD7-4FEA-A60A-C440243B6F22'
DECLARE @DefaultEPAOUserApproveRequest UNIQUEIDENTIFIER = '7142241A-A744-4023-8DB8-E667F6B3F150'
DECLARE @DefaultApplyEPAOStandardSubmission UNIQUEIDENTIFIER = 'A701F4A4-2672-4DA9-8005-E6EEF1025963'
DECLARE @DefaultApplyEPAOAlertSubmission UNIQUEIDENTIFIER = 'A701F4A4-2672-4DA9-8005-E6EEF10455D0'
DECLARE @DefaultEPAOOrganisationDetailsAmended UNIQUEIDENTIFIER = 'F072BF45-8F43-44C7-9117-E8116A5EF388'
DECLARE @DefaultEPAOPermissionsRequested UNIQUEIDENTIFIER = 'DA7603D3-87B1-4040-B272-EDE806538BE8'
DECLARE @DefaultPrintAssessorCoverLetters UNIQUEIDENTIFIER = 'CA7C8BE2-A6F1-479B-A77D-F0614F7E8924'
DECLARE @DefaultEPAOUserApproveConfirm UNIQUEIDENTIFIER = 'F5A787F4-0276-4C23-A125-F6D4C1AE0650'


IF((SELECT COUNT(*) FROM [EMailTemplates] WHERE Id NOT IN
	(
		@DefaultEPAOPermissionsAmended,
		@DefaultApplyEPAOResponse,
		@DefaultApplyEPAOUpdate,
		@DefaultApplyEPAOInitialSubmission,
		@DefaultEPAOUserApproveReject,
		@DefaultEPAOPrimaryContactAmended,
		@DefaultEPAOLoginAccountCreated,
		@DefaultEPAOUserApproveRequest,
		@DefaultApplyEPAOStandardSubmission,
		@DefaultApplyEPAOAlertSubmission,
		@DefaultEPAOOrganisationDetailsAmended,
		@DefaultEPAOPermissionsRequested,
		@DefaultPrintAssessorCoverLetters,
		@DefaultEPAOUserApproveConfirm
	)) > 0)
BEGIN
	BEGIN TRANSACTION

	DECLARE @CurrentEPAOPermissionsAmended UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'EPAOPermissionsAmended')
	DECLARE @CurrentApplyEPAOResponse UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'ApplyEPAOResponse')
	DECLARE @CurrentApplyEPAOUpdate UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'ApplyEPAOUpdate')
	DECLARE @CurrentApplyEPAOInitialSubmission UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'ApplyEPAOInitialSubmission')
	DECLARE @CurrentEPAOUserApproveReject UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'EPAOUserApproveReject')
	DECLARE @CurrentEPAOPrimaryContactAmended UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'EPAOPrimaryContactAmended')
	DECLARE @CurrentEPAOLoginAccountCreated UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'EPAOLoginAccountCreated')
	DECLARE @CurrentEPAOUserApproveRequest UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'EPAOUserApproveRequest')
	DECLARE @CurrentApplyEPAOStandardSubmission UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'ApplyEPAOStandardSubmission')
	DECLARE @CurrentApplyEPAOAlertSubmission UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'ApplyEPAOAlertSubmission')
	DECLARE @CurrentEPAOOrganisationDetailsAmended UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'EPAOOrganisationDetailsAmended')
	DECLARE @CurrentEPAOPermissionsRequested UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'EPAOPermissionsRequested')
	DECLARE @CurrentPrintAssessorCoverLetters UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'PrintAssessorCoverLetters')
	DECLARE @CurrentEPAOUserApproveConfirm UNIQUEIDENTIFIER = (SELECT Id FROM [EMailTemplates] WHERE [TemplateName] = 'EPAOUserApproveConfirm')

	PRINT 'DISABLE FOREIGN KEY CONSTRAINT'
	ALTER TABLE [dbo].[EmailTemplatesRecipients] DROP CONSTRAINT [FK_EmailTemplates_EmailTemplatesRecipients]
	
	PRINT 'UPDATING EMAIL TEMPLATE RECIPIENTS'

	-- update each of the [EmailTemaplateRecipients] to match the known [EmailTemplate] in the default lookup data
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultApplyEPAOAlertSubmission WHERE EmailTemplateId = @CurrentApplyEPAOAlertSubmission AND EmailTemplateId <> @DefaultApplyEPAOAlertSubmission
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultEPAOPermissionsAmended WHERE EmailTemplateId = @CurrentEPAOPermissionsAmended AND EmailTemplateId <> @DefaultEPAOPermissionsAmended
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultApplyEPAOResponse WHERE EmailTemplateId = @CurrentApplyEPAOResponse AND EmailTemplateId <> @DefaultApplyEPAOResponse
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultApplyEPAOUpdate WHERE EmailTemplateId = @CurrentApplyEPAOUpdate AND EmailTemplateId <> @DefaultApplyEPAOUpdate
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultApplyEPAOInitialSubmission WHERE EmailTemplateId = @CurrentApplyEPAOInitialSubmission AND EmailTemplateId <> @DefaultApplyEPAOInitialSubmission
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultEPAOUserApproveReject WHERE EmailTemplateId = @CurrentEPAOUserApproveReject AND EmailTemplateId <> @DefaultEPAOUserApproveReject
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultEPAOPrimaryContactAmended WHERE EmailTemplateId = @CurrentEPAOPrimaryContactAmended AND EmailTemplateId <> @DefaultEPAOPrimaryContactAmended
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultEPAOLoginAccountCreated WHERE EmailTemplateId = @CurrentEPAOLoginAccountCreated AND EmailTemplateId <> @DefaultEPAOLoginAccountCreated
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultEPAOUserApproveRequest WHERE EmailTemplateId = @CurrentEPAOUserApproveRequest AND EmailTemplateId <> @DefaultEPAOUserApproveRequest
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultApplyEPAOStandardSubmission WHERE EmailTemplateId = @CurrentApplyEPAOStandardSubmission AND EmailTemplateId <> @DefaultApplyEPAOStandardSubmission
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultApplyEPAOAlertSubmission WHERE EmailTemplateId = @CurrentApplyEPAOAlertSubmission AND EmailTemplateId <> @DefaultApplyEPAOAlertSubmission
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultEPAOOrganisationDetailsAmended WHERE EmailTemplateId = @CurrentEPAOOrganisationDetailsAmended AND EmailTemplateId <> @DefaultEPAOOrganisationDetailsAmended
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultEPAOPermissionsRequested WHERE EmailTemplateId = @CurrentEPAOPermissionsRequested AND EmailTemplateId <> @DefaultEPAOPermissionsRequested
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultPrintAssessorCoverLetters WHERE EmailTemplateId = @CurrentPrintAssessorCoverLetters AND EmailTemplateId <> @DefaultPrintAssessorCoverLetters
	UPDATE [EmailTemplatesRecipients] SET EmailTemplateId = @DefaultEPAOUserApproveConfirm WHERE EmailTemplateId = @CurrentEPAOUserApproveConfirm AND EmailTemplateId <> @DefaultEPAOUserApproveConfirm
	
	PRINT 'UPDATING EMAIL TEMPLATE'

	-- update each of the [EmailTemplates] to match the known [EmailTemplate] in the default lookup data
	UPDATE [EmailTemplates] SET Id = @DefaultApplyEPAOAlertSubmission WHERE Id = @CurrentApplyEPAOAlertSubmission AND Id <> @DefaultApplyEPAOAlertSubmission
	UPDATE [EmailTemplates] SET Id = @DefaultEPAOPermissionsAmended WHERE Id = @CurrentEPAOPermissionsAmended AND Id <> @DefaultEPAOPermissionsAmended
	UPDATE [EmailTemplates] SET Id = @DefaultApplyEPAOResponse WHERE Id = @CurrentApplyEPAOResponse AND Id <> @DefaultApplyEPAOResponse
	UPDATE [EmailTemplates] SET Id = @DefaultApplyEPAOUpdate WHERE Id = @CurrentApplyEPAOUpdate AND Id <> @DefaultApplyEPAOUpdate
	UPDATE [EmailTemplates] SET Id = @DefaultApplyEPAOInitialSubmission WHERE Id = @CurrentApplyEPAOInitialSubmission AND Id <> @DefaultApplyEPAOInitialSubmission
	UPDATE [EmailTemplates] SET Id = @DefaultEPAOUserApproveReject WHERE Id = @CurrentEPAOUserApproveReject AND Id <> @DefaultEPAOUserApproveReject
	UPDATE [EmailTemplates] SET Id = @DefaultEPAOPrimaryContactAmended WHERE Id = @CurrentEPAOPrimaryContactAmended AND Id <> @DefaultEPAOPrimaryContactAmended
	UPDATE [EmailTemplates] SET Id = @DefaultEPAOLoginAccountCreated WHERE Id = @CurrentEPAOLoginAccountCreated AND Id <> @DefaultEPAOLoginAccountCreated
	UPDATE [EmailTemplates] SET Id = @DefaultEPAOUserApproveRequest WHERE Id = @CurrentEPAOUserApproveRequest AND Id <> @DefaultEPAOUserApproveRequest
	UPDATE [EmailTemplates] SET Id = @DefaultApplyEPAOStandardSubmission WHERE Id = @CurrentApplyEPAOStandardSubmission AND Id <> @DefaultApplyEPAOStandardSubmission
	UPDATE [EmailTemplates] SET Id = @DefaultApplyEPAOAlertSubmission WHERE Id = @CurrentApplyEPAOAlertSubmission AND Id <> @DefaultApplyEPAOAlertSubmission
	UPDATE [EmailTemplates] SET Id = @DefaultEPAOOrganisationDetailsAmended WHERE Id = @CurrentEPAOOrganisationDetailsAmended AND Id <> @DefaultEPAOOrganisationDetailsAmended
	UPDATE [EmailTemplates] SET Id = @DefaultEPAOPermissionsRequested WHERE Id = @CurrentEPAOPermissionsRequested AND Id <> @DefaultEPAOPermissionsRequested
	UPDATE [EmailTemplates] SET Id = @DefaultPrintAssessorCoverLetters WHERE Id = @CurrentPrintAssessorCoverLetters AND Id <> @DefaultPrintAssessorCoverLetters
	UPDATE [EmailTemplates] SET Id = @DefaultEPAOUserApproveConfirm WHERE Id = @CurrentEPAOUserApproveConfirm AND Id <> @DefaultEPAOUserApproveConfirm

	PRINT 'REMOVING INVALID [EmailTemplatesRecipients]'
	DELETE FROM 
		[EmailTemplatesRecipients] 
	WHERE 
		EmailTemplateId NOT IN (SELECT Id FROM [EMailTemplates])

	PRINT 'RECREATE FOREIGN KEY CONSTRAINT'
	ALTER TABLE [dbo].[EmailTemplatesRecipients]  WITH CHECK ADD  CONSTRAINT [FK_EmailTemplates_EmailTemplatesRecipients] FOREIGN KEY([EmailTemplateId])
	REFERENCES [dbo].[EMailTemplates] ([Id])
	
	ALTER TABLE [dbo].[EmailTemplatesRecipients] CHECK CONSTRAINT [FK_EmailTemplates_EmailTemplatesRecipients]
	
	COMMIT TRANSACTION
END
