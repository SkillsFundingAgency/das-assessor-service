/*
	This script will Update recipient of print assessor remove APS add Xerox
*/

BEGIN TRANSACTION

IF (SELECT COUNT(*) FROM [EMailTemplates] WHERE TemplateName = N'PrintAssessorCoverLetters') = 1
BEGIN
	UPDATE [EMailTemplates] SET Recipients='xeroxsupport@funasset.com;epao.helpdesk@education.gov.uk', UpdatedAt= GETDATE() WHERE TemplateName= 'PrintAssessorCoverLetters'
END

-- insert the email template if it does not already exist
 IF NOT EXISTS (SELECT * FROM [EMailTemplates] WHERE TemplateName = N'PrintAssessorCoverLetters')
BEGIN
	INSERT INTO [EMailTemplates] ([TemplateName] ,[TemplateId] ,[Recipients] ,[CreatedAt] ,[DeletedAt] ,[UpdatedAt] ,[RecipientTemplate])
     VALUES ('PrintAssessorCoverLetters' ,'5b171b91-d406-402a-a651-081cce820acb' ,'xeroxsupport@funasset.com;epao.helpdesk@education.gov.uk' , GETDATE() ,NULL ,NULL ,NULL)
END

COMMIT TRANSACTION