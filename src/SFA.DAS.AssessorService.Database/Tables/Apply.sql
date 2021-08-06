CREATE TABLE [Apply](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
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
    [StandardReference] NVARCHAR(10) NULL ,    
 CONSTRAINT [PK_Apply] PRIMARY KEY CLUSTERED 
 (
	[Id] ASC
 )
)
GO


ALTER TABLE [Apply]  
ADD CONSTRAINT [FK_Apply_Organisations_OrganisationId] 
FOREIGN KEY([OrganisationId]) REFERENCES [Organisations] ([Id])
GO


CREATE NONCLUSTERED INDEX IX_APPLY_StandardReference_Status
ON [dbo].[Apply] ([StandardReference],[ApplicationStatus]) INCLUDE (DeletedAT)
GO
