CREATE TABLE [dbo].[Certificates](
	[Id] [uniqueidentifier] NOT NULL,
	[CertificateData] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DeletedBy] [uniqueidentifier] NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](max) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Certificates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_Certificates_Organisations_OrganisationId] FOREIGN KEY(OrganisationId) REFERENCES [dbo].[Organisations] ([Id])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]