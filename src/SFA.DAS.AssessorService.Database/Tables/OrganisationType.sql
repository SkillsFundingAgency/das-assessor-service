CREATE TABLE [OrganisationType]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[Type] [nvarchar](256) NOT NULL, 
	[Status] [nvarchar](10)  NOT NULL,
	[TypeDescription] [nvarchar](500) NULL,
	[FinancialExempt] BIT NOT NULL DEFAULT 0, 
) ON [PRIMARY] 
GO
