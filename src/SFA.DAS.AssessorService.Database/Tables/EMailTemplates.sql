CREATE TABLE [dbo].[EMailTemplates](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[TemplateName] [nvarchar](max) NOT NULL,
	[TemplateId]  [nvarchar](max) NOT NULL,
	[Recipients]  [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,	
	[UpdatedAt] [datetime2](7) NULL,	
   
    CONSTRAINT [PK_EMailTemplates] PRIMARY KEY ([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

