CREATE TABLE [dbo].[EmailTemplatesRecipients]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Recipients]  [nvarchar](max) NULL,
	[EmailTemplateId]  [uniqueidentifier]  NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](256) NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](256) NULL,
	CONSTRAINT [PK_EmailTemplatesRecipients] PRIMARY KEY ([Id]),
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[EmailTemplatesRecipients] 
ADD CONSTRAINT [FK_EmailTemplates_EmailTemplatesRecipients] 
FOREIGN KEY([EmailTemplateId])
REFERENCES [dbo].[EMailTemplates] ([Id]);
GO

ALTER TABLE [dbo].[EmailTemplatesRecipients]
ADD CONSTRAINT [UK_EmailTemplateId] UNIQUE (EmailTemplateId)   
GO  
