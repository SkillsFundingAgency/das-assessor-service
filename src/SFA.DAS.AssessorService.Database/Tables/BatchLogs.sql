CREATE TABLE [dbo].[BatchLogs](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[BatchCreated] [datetime2](7) NOT NULL,
	[BatchNumber] [int] NOT NULL,
	[NumberOfCertificates] [int] NOT NULL,
	[NumberOfCoverLetters] [int] NOT NULL,
	[CertificatesFileName]  [nvarchar](max) NOT NULL,
	[FileUploadStartTime] [datetime2](7) NOT NULL,
	[FileUploadEndTime] [datetime2](7) NULL,
    CONSTRAINT [PK_BatchLogs] PRIMARY KEY ([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

