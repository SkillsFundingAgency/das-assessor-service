﻿CREATE TABLE [dbo].[CertificateLogs](
	[Id] [uniqueidentifier] NOT NULL,
	[Action] [nvarchar](400) NULL,
	[CertificateId] [uniqueidentifier] NOT NULL,
	[EventTime] [datetime2](7) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
 [CertificateData] NVARCHAR(MAX) NOT NULL, 
    [Username] NVARCHAR(256) NOT NULL,
	[BatchNumber] [int] NULL,
    [ReasonForChange] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_CertificateLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CertificateLogs]  ADD  CONSTRAINT [FK_CertificateLogs_Certificates_CertificateId] FOREIGN KEY([CertificateId])
REFERENCES [dbo].[Certificates] ([Id]);
GO

ALTER TABLE [dbo].[CertificateLogs] CHECK CONSTRAINT [FK_CertificateLogs_Certificates_CertificateId]
GO

CREATE NONCLUSTERED INDEX [nci_wi_CertificateLogs_CertificateId_Username] ON [dbo].[CertificateLogs] ([CertificateId], [Username])
INCLUDE ([Action], [EventTime], [Status], [CertificateData], [BatchNumber], [ReasonForChange]) WITH (ONLINE = ON)
GO