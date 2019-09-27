

CREATE TABLE [ExpressionOfInterest](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Email] [nvarchar](256) NOT NULL,
	[ContactName] [nvarchar](250) NOT NULL,
	[OrganisationName] [nvarchar](250) NOT NULL,
	[StandardReference] [nvarchar](10) NULL,
	[Status] [nvarchar](20) NOT NULL DEFAULT 'New',
	[CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [PK_ExpressionofInterest] PRIMARY KEY  
 (
	[Id] ASC
 )
)
