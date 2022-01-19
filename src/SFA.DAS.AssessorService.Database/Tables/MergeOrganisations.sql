CREATE TABLE [dbo].[MergeOrganisations]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[PrimaryEndPointAssessorOrganisationId] [nvarchar](12) NOT NULL,
	[PrimaryEndPointAssessorOrganisationName] [nvarchar](100) NOT NULL,
	[SecondaryEndPointAssessorOrganisationId] [nvarchar](12) NOT NULL,
	[SecondaryEndPointAssessorOrganisationName] [nvarchar](100) NOT NULL,
	[SecondaryEPAOEffectiveTo] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[Status] [nvarchar](11) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[UpdatedBy] [nvarchar](256) NULL,
	[ApprovedAt] [datetime2](7) NULL,
	[ApprovedBy] [nvarchar](256) NULL,
	[CompletedAt] [datetime2](7) NULL,
	[CompletedBy] [nvarchar](256) NULL
	)

 GO

