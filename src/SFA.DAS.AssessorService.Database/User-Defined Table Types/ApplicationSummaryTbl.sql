CREATE TYPE [dbo].[ApplicationSummaryTbl] AS TABLE(
	[ApplicationId] [uniqueidentifier] NULL,
	[SequenceNo] [int] NULL,
	[OrganisationName] [nvarchar](100) NULL,
	[ApplicationType] [nvarchar](10) NULL,
	[StandardName] [nvarchar](500) NULL,
	[StandardCode] [nvarchar](10) NULL,
	[SubmittedDate] [nvarchar](30) NULL,
	[FeedbackAddedDate] [nvarchar](30) NULL,
	[ClosedDate] [nvarchar](30) NULL,
	[SubmissionCount] [int] NULL,
	[ApplicationStatus] [nvarchar](20) NULL,
	[ReviewStatus] [nvarchar](20) NULL,
	[FinancialStatus] [nvarchar](20) NULL,
	[FinancialGrade] [nvarchar](20) NULL,
	[SequenceStatus] [nvarchar](20)
)