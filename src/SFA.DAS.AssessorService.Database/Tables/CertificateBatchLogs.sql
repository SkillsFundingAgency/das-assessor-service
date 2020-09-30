CREATE TABLE [dbo].[CertificateBatchLogs]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[CertificateReference] VARCHAR(50) NOT NULL,
	[BatchNumber] [int] NOT NULL,
	[CertificateData] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[StatusAt] [datetime2](7) NOT NULL,
	[ReasonForChange] NVARCHAR(MAX) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](256) NULL, 
	[DeletedAt] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](256) NULL,
	CONSTRAINT [PK_CertificateBatchLogs] PRIMARY KEY CLUSTERED 
	(
		[CertificateReference],
		[BatchNumber]
	)
)
GO

ALTER TABLE [dbo].[CertificateBatchLogs] ADD CONSTRAINT [FK_CertificateBatchLogs_BatchLogs] FOREIGN KEY([BatchNumber])
REFERENCES [dbo].[BatchLogs] ([BatchNumber])
GO

ALTER TABLE [dbo].[CertificateBatchLogs] CHECK CONSTRAINT [FK_CertificateBatchLogs_BatchLogs];
GO

