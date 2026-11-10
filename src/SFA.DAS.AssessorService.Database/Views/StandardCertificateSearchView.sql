CREATE VIEW [dbo].[StandardCertificateSearchView]
AS
SELECT
    ce.[CertificateReferenceId],
    ce.[CertificateFamilyName],
    ce.[DateOfBirth],
    ce.[Uln],
    ce.[CourseCode],
    ce.[CourseName],
    ce.[CourseLevel],
    ce.[DateAwarded],
    ce.[ProviderName],
    ce.[Ukprn],
    stn.[Route] AS [Sector],
    ce.[CreateDay]
FROM [dbo].[StandardCertificateSearchDataView] ce WITH (NOEXPAND)
    JOIN [dbo].[Standards] stn
    ON stn.[StandardUId] = ce.[StandardUId];
GO