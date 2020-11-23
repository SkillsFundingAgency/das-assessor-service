CREATE TABLE [dbo].[EMailTemplates](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[TemplateName] [nvarchar](100) NOT NULL,
	[TemplateId]  [nvarchar](100) NOT NULL,
    CONSTRAINT [PK_EMailTemplates] PRIMARY KEY ([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE UNIQUE INDEX [IXU_EMailTemplates] ON [EMailTemplates] ([TemplateName])
GO