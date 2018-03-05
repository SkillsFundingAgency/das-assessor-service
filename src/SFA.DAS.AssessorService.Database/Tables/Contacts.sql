CREATE TABLE [dbo].[Contacts](
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DisplayName] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[EndPointAssessorOrganisationId] [nvarchar](max) NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[Username] [nvarchar](450) NOT NULL,
	[Status] [nvarchar](max) NULL,
 CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Contacts_Username] UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_Contacts_Organisations_OrganisationId] FOREIGN KEY(OrganisationId) REFERENCES [dbo].[Organisations] ([Id])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]