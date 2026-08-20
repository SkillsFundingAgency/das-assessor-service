CREATE VIEW [dbo].[FrameworkCertificateSearchView]
WITH SCHEMABINDING
AS
SELECT 
    [ApprenticeId] [CertificateReferenceId],
    [CertificateFamilyName] [CertificateFamilyName],
    [ApprenticeDoB] [DateOfBirth],
    [ApprenticeULN] [Uln],
    [TrainingCode] [CourseCode],
    [FrameworkName] [CourseName],
    [PathwayName],
    [ApprenticeshipLevelName] [CourseLevel],
    [CertificationDate] [DateAwarded],
    [ProviderName],
    [Ukprn],
    [CreatedOn] [CreateDay]
FROM [dbo].[FrameworkLearner]
WHERE [ApprenticeULN] IS NOT NULL
GO

CREATE UNIQUE CLUSTERED INDEX PK_FrameworkCertificateSearchView
ON [dbo].[FrameworkCertificateSearchView] ([CreateDay],[CertificateReferenceId])
GO

CREATE NONCLUSTERED INDEX IX_FrameworkCertificateSearchView_Masks_CreateDay
ON [dbo].[FrameworkCertificateSearchView] ([CreateDay],[CourseCode],[ProviderName],[DateAwarded],[Ukprn])
INCLUDE ([CourseName],[CourseLevel],[Uln])
GO

CREATE NONCLUSTERED INDEX IX_FrameworkCertificateSearchView
ON [dbo].[FrameworkCertificateSearchView] ([DateOfBirth],[CertificateFamilyName],[Uln])
INCLUDE ([CourseCode],[CourseName],[CourseLevel],[DateAwarded],[ProviderName],[Ukprn])
GO

CREATE NONCLUSTERED INDEX IX_FrameworkCertificateSearchView_Masks_Uln
ON [dbo].[FrameworkCertificateSearchView] ([CourseCode],[ProviderName],[Uln])
GO
