CREATE TABLE [dbo].[FrameworkLearner]
(
 [Id] UNIQUEIDENTIFIER NOT NULL
,[CertificateReference] BIGINT NOT NULL
,[CertificationYear] VARCHAR(4) NOT NULL
,[CertificationDate] DATE NOT NULL
,[FrameworkName] NVARCHAR(110) NOT NULL
,[ApprenticeTitle] NVARCHAR(10)
,[ApprenticeFullname] NVARCHAR(100) NOT NULL
,[ApprenticeSurname] NVARCHAR(40) NOT NULL
,[ApprenticeForename] NVARCHAR(30) NOT NULL
,[ApprenticeMiddlename] NVARCHAR(30) NULL
,[ApprenticeDoB] DATE NOT NULL
,[ApprenticeUniqueNumber] BIGINT NULL
,[CentreName] NVARCHAR(100) NULL
,[IssuingAuthority] NVARCHAR(50) NULL
,[ProviderName] NVARCHAR(120) NOT NULL
,[Ukprn] VARCHAR(10) NULL
,[EmployerName] NVARCHAR(100) NULL
,[CompetenceQualificationId] INT NULL
,[CompetenceQualification] NVARCHAR(200) NULL
,[CompetenceQualificationNumber] NVARCHAR(50) NULL
,[CompetanceAwardingBody] NVARCHAR(100) NULL
,[KnowledgeQualificationId] INT NULL
,[KnowledgeQualification] NVARCHAR(200) NULL
,[KnowledgeQualificationNumber] NVARCHAR(50) NULL
,[KnowledgeAwardingBody] NVARCHAR(100) NULL
,[CombinedQualificationId] INT NULL
,[CombinedQualification] NVARCHAR(200) NULL
,[CombinedQualificationNumber] NVARCHAR(50) NULL
,[CombinedAwardingBody] NVARCHAR(100) NULL
,[ApprenticeStartdate] DATE NULL
,[ApprenticeEnddate] DATE NULL
,[ApprenticeLastdateInLearning] DATE NULL
,[ApprenticeshipSector] NVARCHAR(200) NULL
,[EnglishId] INT NULL
,[EnglishSkill] NVARCHAR(220) NULL
,[MathsId] INT NULL
,[MathsSkill] NVARCHAR(220) NULL
,[IctId] INT NULL
,[IctSkill] NVARCHAR(220) NULL
,[Framework] NVARCHAR(110) NOT NULL
,[ApprenticeshipPathway] NVARCHAR(150) NOT NULL
,[ApprenticeshipLevel] NVARCHAR(20) NOT NULL
,[ApprenticeId] BIGINT NOT NULL
,[CreatedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
,[ApprenticeNameMatch] NVARCHAR(70) NOT NULL
,CONSTRAINT [PK_FrameworkLearner] PRIMARY KEY CLUSTERED ([Id])
);
GO
CREATE INDEX [IX_FrameworkLearner] ON [dbo].[FrameworkLearner] 
([ApprenticeSurname],[ApprenticeForename], [ApprenticeDoB], [CertificationYear], [Framework], [ApprenticeshipPathway], [ApprenticeshipLevel]) 
INCLUDE ([Id]);
GO
CREATE UNIQUE INDEX [IXU_FrameworkLearner_NameMatch] ON [dbo].[FrameworkLearner] 
([ApprenticeNameMatch], [ApprenticeDoB], [CertificationYear], [Framework], [ApprenticeshipPathway], [ApprenticeshipLevel]
,[CompetenceQualificationId], [KnowledgeQualificationId], [CombinedQualificationId])
INCLUDE ([Id],[ApprenticeSurname],[ApprenticeForename]);
GO
