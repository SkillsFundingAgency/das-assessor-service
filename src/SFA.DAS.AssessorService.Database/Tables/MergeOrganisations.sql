CREATE TABLE [dbo].[MergeOrganisations]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[PrimaryEndPointAssessorOrganisationId] [nvarchar](12) NOT NULL,
	[SecondaryEndPointAssessorOrganisationId] [nvarchar](12) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[Status] [nvarchar](10) NOT NULL,
	[CreatedBy] [UNIQUEIDENTIFIER] NOT NULL,
	[UpdatedBy] [UNIQUEIDENTIFIER] NULL,
	[ApprovedAt] [datetime2](7) NULL,
	[ApprovedBy] [UNIQUEIDENTIFIER] NULL,

 
 CONSTRAINT [AK_MergeOrganisations_PrimaryEndPointAssessorOrganisationId] UNIQUE NONCLUSTERED 
(
	[PrimaryEndPointAssessorOrganisationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 
CONSTRAINT [AK_MergeOrganisations_SecondaryEndPointAssessorOrganisationId] UNIQUE NONCLUSTERED 
(
	[SecondaryEndPointAssessorOrganisationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


GO

