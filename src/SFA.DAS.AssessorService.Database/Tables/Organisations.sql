CREATE TABLE [dbo].[Organisations](
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[EndPointAssessorName] [nvarchar](100) NOT NULL,
	[EndPointAssessorOrganisationId] [nvarchar](12) NOT NULL,
	[EndPointAssessorUkprn] [int] NULL,
	[PrimaryContact] [nvarchar](30) NULL,
	[Status] [nvarchar](10) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[OrganisationTypeId] [int] NULL,  
	OrganisationData [nvarchar](max) NULL,
	[ApiEnabled] [bit] NOT NULL DEFAULT 0,
	[ApiUser] [nvarchar](100) NULL,

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

ALTER TABLE [dbo].[Organisations] ADD CONSTRAINT [FK_OrganisationType_Organisations] FOREIGN KEY([OrganisationTypeId])
REFERENCES [dbo].[OrganisationType] ([Id]);
GO
 ALTER TABLE [dbo].[Organisations] CHECK CONSTRAINT [FK_OrganisationType_Organisations];
GO