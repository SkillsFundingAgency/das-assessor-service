CREATE TABLE [dbo].[Certificates](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[CertificateData] [nvarchar](max) NOT NULL,
	[ToBePrinted] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](256) NULL,
	[CertificateReference] NVARCHAR(50) NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
	[BatchNumber] [int] NULL,
	[Status] [nvarchar](20) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](256) NULL, 
    [Uln] BIGINT NOT NULL, 
    [StandardCode] INT NOT NULL, 
    [ProviderUkPrn] INT NOT NULL, 
    [CertificateReferenceId] INT NOT NULL IDENTITY(10001,1), 
	[LearnRefNumber] NVARCHAR(12) NULL,
	[CreateDay] DATE NOT NULL,
	[IsPrivatelyFunded] BIT, 
	[PrivatelyFundedStatus] NVARCHAR(20) NULL, 
    [StandardUId] VARCHAR(20)  NULL ,
	
	[LearnerFamilyName] as CAST(JSON_VALUE(CertificateData, '$.LearnerFamilyName') AS NVARCHAR(255)),
	[LearnerGivenNames] as CAST(JSON_VALUE(CertificateData, '$.LearnerGivenNames') AS NVARCHAR(255)),
	[LearnerFullNameNoSpaces] as CAST(REPLACE(JSON_VALUE(CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(CertificateData, '$.LearnerFamilyName'),' ','') AS NVARCHAR(255)),
	[FullName] as CAST(JSON_VALUE(CertificateData, '$.FullName') AS NVARCHAR(255)),
	[ContactOrganisation] as CAST(JSON_VALUE(CertificateData, '$.ContactOrganisation') AS NVARCHAR(255)),
	[ProviderName] as CAST(JSON_VALUE(CertificateData, '$.ProviderName') AS NVARCHAR(255)),
	[ContactName] as CAST(JSON_VALUE(CertificateData, '$.ContactName') AS NVARCHAR(255)),
	[CourseOption] as CAST(JSON_VALUE(CertificateData, '$.CourseOption') AS NVARCHAR(255)),
	[OverallGrade] as CAST(JSON_VALUE(CertificateData, '$.OverallGrade') AS NVARCHAR(255)),
	[StandardReference] as CAST(JSON_VALUE(CertificateData, '$.StandardReference') AS NVARCHAR(255)),
	[StandardName] as CAST(JSON_VALUE(CertificateData, '$.StandardName') AS NVARCHAR(255)),
	[Version] as CAST(JSON_VALUE(CertificateData, '$.Version') AS NVARCHAR(255)),
	[StandardLevel] as CAST(JSON_VALUE(CertificateData, '$.StandardLevel') AS INT),
	[AchievementDate] as CAST(JSON_VALUE(CertificateData, '$.AchievementDate') AS [datetime2](7)),
	[LearningStartDate] as CAST(JSON_VALUE(CertificateData, '$.LearningStartDate') AS [datetime2](7)),
	[ContactAddLine1] as CAST(JSON_VALUE(CertificateData, '$.ContactAddLine1') AS NVARCHAR(255)),
	[ContactAddLine2] as CAST(JSON_VALUE(CertificateData, '$.ContactAddLine2') AS NVARCHAR(255)),
	[ContactAddLine3] as CAST(JSON_VALUE(CertificateData, '$.ContactAddLine3') AS NVARCHAR(255)),
	[ContactAddLine4] as CAST(JSON_VALUE(CertificateData, '$.ContactAddLine4') AS NVARCHAR(255)),
	[ContactPostCode] as CAST(JSON_VALUE(CertificateData, '$.ContactPostCode') AS NVARCHAR(255)),
	[LatestEPAOutcome] as CAST(JSON_VALUE(CertificateData, '$.EpaDetails.LatestEpaOutcome') AS NVARCHAR(15)),

    CONSTRAINT [PK_Certificates] PRIMARY KEY ([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Certificates]  ADD  CONSTRAINT [FK_Certificates_Organisations_OrganisationId] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisations] ([Id]);
GO
 ALTER TABLE [dbo].[Certificates] CHECK CONSTRAINT [FK_Certificates_Organisations_OrganisationId]
GO

ALTER TABLE [dbo].[Certificates]  ADD  CONSTRAINT [FK_Certificates_CertificateBatchLogs] FOREIGN KEY([CertificateReference], [BatchNumber])
REFERENCES [dbo].[CertificateBatchLogs] ([CertificateReference], [BatchNumber])
GO

ALTER TABLE [dbo].[Certficates] CHECK CONSTRAINT [FK_Certificates_CertificateBatchLogs]
GO

CREATE UNIQUE INDEX [IXU_Certificates] ON [Certificates] ([Uln], [StandardCode])
GO

CREATE INDEX [IX_Certificates_CreateDay] ON [Certificates] ([CreateDay]) INCLUDE ([Status], [CertificateData])
GO

CREATE INDEX [IX_Certificates_CertificateReference] ON [Certificates] ([CertificateReference]) INCLUDE ([Id])
GO

CREATE INDEX [IX_Certificates_OrganisationId] ON [Certificates] ([OrganisationId]) INCLUDE ([CreatedAt], [UpdatedAt], [CertificateData], [Status])
GO

CREATE INDEX [IX_Certificates_LearnerNames] ON [Certificates] ([LearnerFamilyName],[LearnerGivenNames],[LearnerFullNameNoSpaces])
GO

CREATE INDEX [IX_Certificates_Search] ON [Certificates] ([FullName],[ContactOrganisation],[ProviderName])
GO