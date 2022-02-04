CREATE TABLE [dbo].[MergeApply]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[MergeOrganisationId] int NOT NULL,
    [ApplyId] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
	[ApplicationStatus] [nvarchar](20) NOT NULL DEFAULT 'New',
	[ReviewStatus] [nvarchar](20) NOT NULL DEFAULT 'Draft',
	[ApplyData] [nvarchar](max) NULL,
	[FinancialReviewStatus] [nvarchar](20) NOT NULL DEFAULT 'Required',
	[FinancialGrade] [nvarchar](max) NULL,
	[StandardCode] int NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](256) NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](256) NULL,
    [StandardApplicationType]  NVARCHAR(60) NULL ,  
    [StandardReference] NVARCHAR(10) NULL,
	[Replicates] NVARCHAR (6) NOT NULL

)
GO