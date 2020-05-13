CREATE TABLE [dbo].[CertificateLogs](
	[Id] [uniqueidentifier] NOT NULL,
	[Action] [nvarchar](400) NULL,
	[CertificateId] [uniqueidentifier] NOT NULL,
	[EventTime] [datetime2](7) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[CertificateData] NVARCHAR(MAX) NOT NULL, 
	[Username] NVARCHAR(256) NOT NULL,
	[BatchNumber] [int] NULL,
	[ReasonForChange] NVARCHAR(MAX) NULL,
	[LatestEpaOutcome] AS JSON_VALUE([CertificateData],'$.EpaDetails.LatestEpaOutcome'),
	CONSTRAINT [PK_CertificateLogs] PRIMARY KEY NONCLUSTERED 
	(
		[Id] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CertificateLogs]  ADD  CONSTRAINT [FK_CertificateLogs_Certificates_CertificateId] FOREIGN KEY([CertificateId])
REFERENCES [dbo].[Certificates] ([Id]);
GO

ALTER TABLE [dbo].[CertificateLogs] CHECK CONSTRAINT [FK_CertificateLogs_Certificates_CertificateId]
GO

-- the clustered index is not the primary key to reduce fragmentation of non-sequential uniqueidentifier
CREATE CLUSTERED INDEX [ICX_CertificateLogs_EventTime] ON [dbo].[CertificateLogs] ([EventTime])
WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_CertificateLogs_CertificateId_Username] ON [dbo].[CertificateLogs] ([CertificateId], [Username])
INCLUDE ([Action], [EventTime], [Status], [CertificateData], [BatchNumber], [ReasonForChange]) 
WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_CertficateLogs_Status_Username] ON [dbo].[CertificateLogs] ([Status], [Username]) 
WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_CertificateLogs_Action_EventTime] ON [dbo].[CertificateLogs] ([Action], [EventTime])
INCLUDE ( [CertificateId], [Status], [CertificateData]) 
WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_CertificateLogs_Action_CertificateId] ON [dbo].[CertificateLogs] ([Action], [CertificateId])
INCLUDE ( [EventTime], [Status], [LatestEpaOutcome], [CertificateData]) 
WITH (ONLINE = ON)
GO