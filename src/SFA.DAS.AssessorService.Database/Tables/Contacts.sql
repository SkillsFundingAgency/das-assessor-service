CREATE TABLE [dbo].[Contacts](
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DisplayName] [nvarchar](120) NOT NULL,
	[Email] [nvarchar](120) NULL,
	[EndPointAssessorOrganisationId] [nvarchar](12) NOT NULL,
	[OrganisationId] [uniqueidentifier] NULL,
	[Status] [nvarchar](10) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[Username] [nvarchar](30) NOT NULL,
	[PhoneNumber] [NVARCHAR] (50) NULL
 CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    CONSTRAINT [AK_Contacts_Username] UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[Contacts] ADD CONSTRAINT [FK_Contacts_Organisations_OrganisationId] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisations] ([Id]);
GO
 ALTER TABLE [dbo].[Contacts] CHECK CONSTRAINT [FK_Contacts_Organisations_OrganisationId];
GO
