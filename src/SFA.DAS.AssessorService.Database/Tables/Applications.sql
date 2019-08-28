CREATE TABLE [Applications](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
	[ApplicationStatus] [nvarchar](20) NOT NULL,
	[ApplicationData] [nvarchar](max) NULL,
	[FinancialGrade] [nvarchar](max) NULL,
	[StandardCode] int NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](256) NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](256) NULL,
 CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED 
 (
	[Id] ASC
 )
)
GO


ALTER TABLE [Applications]  
ADD CONSTRAINT [FK_Applications_Organisations_OrganisationId] 
FOREIGN KEY([OrganisationId]) REFERENCES [Organisations] ([Id])
GO
