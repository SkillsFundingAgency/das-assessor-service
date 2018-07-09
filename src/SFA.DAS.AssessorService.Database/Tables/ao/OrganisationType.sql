CREATE TABLE [ao].[OrganisationType]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[OrganisationType] [nvarchar](256) NOT NULL,
	CONSTRAINT [PK_OrganisationTypes] PRIMARY KEY ([Id]),
) ON [PRIMARY] 
GO