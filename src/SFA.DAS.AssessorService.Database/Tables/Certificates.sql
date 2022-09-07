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
	
	[LearnerFamilyName] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.LearnerFamilyName')),
	[LearnerGivenNames] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.LearnerGivenNames')),
	[LearnerFullNameNoSpaces] as CONVERT(NVARCHAR(255),REPLACE(JSON_VALUE(CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(CertificateData, '$.LearnerFamilyName'),' ','')),
	[FullName] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.FullName')),
	[ContactOrganisation] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.ContactOrganisation')),
	[ProviderName] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.ProviderName')),
	[ContactName] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.ContactName')),
	[CourseOption] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.CourseOption')),
	[OverallGrade] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.OverallGrade')),
	[StandardReference] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.StandardReference')),
	[StandardName] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.StandardName')),
	[Version] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.Version')),
	[StandardLevel] as CONVERT(INT,JSON_VALUE(CertificateData, '$.StandardLevel')),
	[AchievementDate] as CONVERT([datetime2](7),JSON_VALUE(CertificateData, '$.AchievementDate')),
	[LearningStartDate] as CONVERT([datetime2](7),JSON_VALUE(CertificateData, '$.LearningStartDate')),
	[ContactAddLine1] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.ContactAddLine1')),
	[ContactAddLine2] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.ContactAddLine2')),
	[ContactAddLine3] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.ContactAddLine3')),
	[ContactAddLine4] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.ContactAddLine4')),
	[ContactPostCode] as CONVERT(NVARCHAR(255),JSON_VALUE(CertificateData, '$.ContactPostCode')),
	[LatestEPAOutcome] as CONVERT(NVARCHAR(15),JSON_VALUE(CertificateData, '$.EpaDetails.LatestEpaOutcome')),

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