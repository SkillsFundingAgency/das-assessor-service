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

 GO

