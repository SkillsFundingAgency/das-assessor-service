BEGIN TRANSACTION

IF ((SELECT COUNT(*) FROM dbo.[EMailTemplates]  WHERE RecipientTemplate = 'ApplyEPAOAlertSubmission' AND TemplateName = 'ApplyEPAOInitialSubmission') = 1)
BEGIN
    UPDATE dbo.[EMailTemplates] SET RecipientTemplate = NULL WHERE RecipientTemplate = 'ApplyEPAOAlertSubmission' AND TemplateName = 'ApplyEPAOInitialSubmission'
END


IF ((SELECT COUNT(*) FROM dbo.[EMailTemplates]  WHERE TemplateName = 'ApplyEPAOInitialSubmission' AND TemplateId = '68410850-909b-4669-a60a-f60e4b1cb89f') = 1)
BEGIN

 UPDATE dbo.[EMailTemplates] 
 SET Recipients = (SELECT Recipients FROM dbo.[EMailTemplates]  WHERE TemplateName = 'ApplyEPAOAlertSubmission' and TemplateId ='a56c47c8-6310-4f5c-a3f6-9e996c375557')
 WHERE  TemplateName = 'ApplyEPAOInitialSubmission' AND TemplateId = '68410850-909b-4669-a60a-f60e4b1cb89f' 

END

COMMIT TRANSACTION