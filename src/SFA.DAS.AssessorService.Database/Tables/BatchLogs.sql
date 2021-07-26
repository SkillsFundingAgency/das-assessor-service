CREATE TABLE [dbo].[BatchLogs](
	[Id] [uniqueidentifier] NOT NULL,
	[Period] [nvarchar](4) NOT NULL,
	[BatchCreated] [datetime2](7) NULL,
	[ScheduledDate][datetime2](7) NOT NULL,
	[BatchNumber] [int] NOT NULL,
	[NumberOfCertificates] [int] NULL,
	[NumberOfCoverLetters] [int] NULL,
	[CertificatesFileName]  [nvarchar](max) NULL,
	[FileUploadStartTime] [datetime2](7) NULL,
	[FileUploadEndTime] [datetime2](7) NULL,
	[BatchData] [nvarchar](max) NULL,
    CONSTRAINT [PK_BatchLogs] PRIMARY KEY ([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [BatchLogs] ADD CONSTRAINT [DF_BatchLogs_Id] DEFAULT NEWID() FOR [Id]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_BatchLogs_BatchNumber] ON [dbo].[BatchLogs]([BatchNumber]) 
GO
