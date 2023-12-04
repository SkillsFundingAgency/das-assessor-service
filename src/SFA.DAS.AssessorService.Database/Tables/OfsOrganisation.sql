CREATE TABLE [dbo].[OfsOrganisation]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Ukprn] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETUTCDATE()
 CONSTRAINT [PK_OfsOrganisation] PRIMARY KEY CLUSTERED ( [Id] ASC )
)
GO

CREATE UNIQUE INDEX [IXU_OfsOrganisation_Ukprn] ON [OfsOrganisation] ([Ukprn])
GO