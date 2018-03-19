﻿CREATE TABLE [dbo].[Certificates](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[CertificateData] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](30) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](30) NULL,
	[CertificateReference] VARCHAR(50) NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](30) NULL, 
    [Uln] BIGINT NOT NULL, 
    [StandardCode] INT NOT NULL, 
    [ProviderUkPrn] INT NOT NULL, 
    CONSTRAINT [PK_Certificates] PRIMARY KEY ([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Certificates]  WITH CHECK ADD  CONSTRAINT [FK_Certificates_Organisations_OrganisationId] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisations] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Certificates] CHECK CONSTRAINT [FK_Certificates_Organisations_OrganisationId]
GO