CREATE TABLE [dbo].[BatchLogs](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Period] [nvarchar](4) NOT NULL,
	[BatchCreated] [datetime2](7) NOT NULL,
	[ScheduledDate][datetime2](7) NOT NULL,
	[BatchNumber] [int] NOT NULL,
	[NumberOfCertificates] [int] NOT NULL,
	[NumberOfCoverLetters] [int] NOT NULL,
	[CertificatesFileName]  [nvarchar](max) NOT NULL,
	[FileUploadStartTime] [datetime2](7) NOT NULL,
	[FileUploadEndTime] [datetime2](7) NULL,
		[BatchData] [nvarchar](max) NULL,
    CONSTRAINT [PK_BatchLogs] PRIMARY KEY ([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

