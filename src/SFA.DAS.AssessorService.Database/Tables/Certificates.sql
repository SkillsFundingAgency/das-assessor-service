CREATE TABLE [dbo].[Certificates](
	[Id] [uniqueidentifier] NOT NULL,
	[CertificateData] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](30) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](30) NULL,
	[EndPointAssessorCertificateId] [int] NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](30) NULL,
 CONSTRAINT [PK_Certificates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Certificates]  WITH CHECK ADD  CONSTRAINT [FK_Certificates_Organisations_OrganisationId] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisations] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Certificates] CHECK CONSTRAINT [FK_Certificates_Organisations_OrganisationId]
GO