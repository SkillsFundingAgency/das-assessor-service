CREATE TABLE [dbo].[StandardNonApprovedCollation]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceNumber] [nvarchar](10) NULL,
	[Title] [nvarchar](500) NOT NULL,
	[StandardData] [nvarchar](max) NULL,
	[DateAdded] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
	[DateRemoved] [datetime] NULL,
	[IsLive] [bit] NULL,
	CONSTRAINT [PK_StandardNonApprovedCollation] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO