CREATE TABLE [dbo].[CertificateLogs](
	[Id] [uniqueidentifier] NOT NULL,
	[Action] [nvarchar](max) NULL,
	[CertificateId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[EventTime] [datetime2](7) NOT NULL,
	[Status] [nvarchar](max) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	 CONSTRAINT [PK_CertificateLogs] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_CertificateLogs_Certificates_CertificateId] FOREIGN KEY(CertificateId) REFERENCES [dbo].[Certificates] ([Id])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
