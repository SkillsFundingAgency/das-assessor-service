CREATE TABLE [OrganisationType]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[Type] [nvarchar](256) NOT NULL, 
	[Status] [nvarchar](10)  NOT NULL,
   [TypeDescription] [nvarchar](500) NULL, 
) ON [PRIMARY] 
GO
