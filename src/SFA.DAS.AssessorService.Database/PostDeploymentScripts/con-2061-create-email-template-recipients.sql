BEGIN TRANSACTION

IF ((SELECT COUNT(*) FROM [dbo].[EmailTemplatesRecipients]) = 0)
BEGIN
        INSERT INTO [dbo].[EmailTemplatesRecipients] (Recipients, EmailTemplateId, CreatedAt, CreatedBy)
        SELECT Recipients, Id, GETUTCDATE(), 'admin' FROM [dbo].[EMailTemplates]
        ORDER BY CreatedAt DESC
END

COMMIT TRANSACTION