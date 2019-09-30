

CREATE TABLE [ExpressionsOfInterest](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Email] [nvarchar](256) NOT NULL,
	[ContactName] [nvarchar](250) NULL,
	[ContactPhone] [nvarchar](250) NULL,
	[OrganisationName] [nvarchar](250) NOT NULL,
	[StandardReference] [nvarchar](10) NULL,
	[Status] [nvarchar](20) NOT NULL DEFAULT 'New',
	[CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [PK_ExpressionsofInterest] PRIMARY KEY  
 (
	[Id] ASC
 )
)
