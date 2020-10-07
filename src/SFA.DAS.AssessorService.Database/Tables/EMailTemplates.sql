CREATE TABLE [dbo].[EMailTemplates](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[TemplateName] [nvarchar](100) NOT NULL,
	[TemplateId]  [nvarchar](100) NOT NULL,
	[Recipients]  [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
	[DeletedAt] [datetime2](7) NULL,	
	[UpdatedAt] [datetime2](7) NULL,	
   
    [RecipientTemplate] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_EMailTemplates] PRIMARY KEY ([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE UNIQUE INDEX [IXU_EMailTemplates] ON [EMailTemplates] ([TemplateName])
GO