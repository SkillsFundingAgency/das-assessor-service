﻿CREATE TABLE [dbo].[Organisations](
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[EndPointAssessorName] [nvarchar](100) NOT NULL,
	[EndPointAssessorOrganisationId] [nvarchar](12) NOT NULL,
	[EndPointAssessorUkprn] [int] NULL,
	[PrimaryContact] [nvarchar](30) NULL,
	[Status] [nvarchar](10) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[WebsiteLink] [nvarchar](256) NULL,
	[OrganisationTypeId] [int] NULL,  
	[LegalName] [nvarchar](256) NULL,
	[Address1] [NVARCHAR](50) NULL,
	[Address2] [nvarchar](50) NULL,
	[Address3] [nvarchar](50) NULL,
	[Address4] [nvarchar](50) NULL,
	[Postcode] [nvarchar](8) NULL,
 CONSTRAINT [PK_Organisations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Organisations_EndPointAssessorOrganisationId] UNIQUE NONCLUSTERED 
(
	[EndPointAssessorOrganisationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO