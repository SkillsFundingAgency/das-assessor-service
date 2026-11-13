CREATE VIEW [dbo].[StandardCertificateSearchDataView]
WITH SCHEMABINDING
AS
SELECT
    [CertificateReferenceId],
    [CertificateFamilyName],
    [DateOfBirth],
    [Uln],
    [StandardUId],
    CONVERT(VARCHAR(8), [StandardCode]) AS [CourseCode],
    [StandardName] AS [CourseName],
    CONVERT(VARCHAR(20), [StandardLevel]) AS [CourseLevel],
    [AchievementDate] AS [DateAwarded],
    [ProviderName],
    [ProviderUkPrn] AS [Ukprn],
    [CreateDay]
FROM [dbo].[Certificates]
WHERE [Type] = N'Standard'
    AND [Status] NOT IN ('Draft', 'Deleted')
    AND [LatestEPAOutcome] = 'Pass'
    AND [Uln] > 1000000000
    AND [Uln] < 9999999999
    AND [DateOfBirth] IS NOT NULL;
GO

CREATE UNIQUE CLUSTERED INDEX [PK_StandardCertificateSearchDataView]
ON [dbo].[StandardCertificateSearchDataView] ([CertificateReferenceId]);
GO

CREATE NONCLUSTERED INDEX [IX_StandardCertificateSearchDataView]
ON [dbo].[StandardCertificateSearchDataView] ([DateOfBirth], [CertificateFamilyName], [Uln])
INCLUDE ([StandardUId], [CourseCode], [Ukprn], [ProviderName], [CourseName], [CourseLevel], [DateAwarded], [CreateDay]);
GO

CREATE NONCLUSTERED INDEX [IX_StandardCertificateSearchDataView_Masks]
ON [dbo].[StandardCertificateSearchDataView] ([Uln], [CourseCode], [Ukprn])
INCLUDE ([StandardUId]);
GO

CREATE NONCLUSTERED INDEX [IX_StandardCertificateSearchDataView_Masks_Uln]
ON [dbo].[StandardCertificateSearchDataView] ([CourseCode], [Ukprn], [CreateDay], [CertificateReferenceId], [Uln])
INCLUDE ([StandardUId], [CourseName], [CourseLevel], [ProviderName]);
GO