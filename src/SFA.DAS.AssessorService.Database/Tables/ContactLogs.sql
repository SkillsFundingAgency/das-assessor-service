CREATE TABLE [dbo].[ContactLogs](
	[DateTime] [datetime2](7) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ContactId] [uniqueidentifier] NOT NULL,
	[ContactLogType] [nvarchar](50) NOT NULL,
	[ContactLogDetails] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
