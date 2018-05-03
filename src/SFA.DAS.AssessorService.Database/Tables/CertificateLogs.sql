CREATE TABLE [dbo].[CertificateLogs](
	[Id] [uniqueidentifier] NOT NULL,
	[Action] [nvarchar](400) NOT NULL,
	[CertificateId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[EventTime] [datetime2](7) NOT NULL,
	[Status] [nvarchar](12) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_CertificateLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_CertificateLogs_EventTime] UNIQUE NONCLUSTERED 
(
	[EventTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CertificateLogs]  ADD  CONSTRAINT [FK_CertificateLogs_Certificates_CertificateId] FOREIGN KEY([CertificateId])
REFERENCES [dbo].[Certificates] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CertificateLogs] CHECK CONSTRAINT [FK_CertificateLogs_Certificates_CertificateId]
GO