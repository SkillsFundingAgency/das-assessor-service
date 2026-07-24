CREATE VIEW [dbo].[StandardCertificateSearchView]
WITH SCHEMABINDING
AS
SELECT [CertificateReferenceId]
,[CertificateFamilyName]
,[DateOfBirth]
,[Uln]
,CONVERT(varchar(8),[StandardCode]) AS [CourseCode]
,[StandardName] AS [CourseName]
,CONVERT(varchar(20),[StandardLevel]) AS [CourseLevel]
,[AchievementDate] AS [DateAwarded]
,[ProviderName]
,[ProviderUkPrn] AS [Ukprn]
,st1.[Route] [Sector]
,[CreateDay]
FROM [dbo].[Certificates] ce1
JOIN [dbo].[Standards] st1 on st1.[StandardUId] = ce1.[StandardUId]
WHERE [Type] = N'Standard' 
AND ce1.[Status] NOT IN ('draft','deleted')
AND [LatestEPAOutcome] = 'PASS'
AND ([Uln] > 1000000000 AND [Uln] < 9999999999) 
AND [DateOfBirth] IS NOT NULL
;
GO

CREATE UNIQUE CLUSTERED INDEX PK_StandardCertificateSearchView
ON [dbo].[StandardCertificateSearchView] ([CertificateReferenceId]);
GO

CREATE NONCLUSTERED INDEX IX_StandardCertificateSearchView
ON [dbo].[StandardCertificateSearchView] ([DateOfBirth],[CertificateFamilyName],[Uln])
INCLUDE ([CourseCode],[UkPrn],[ProviderName],[CourseName], [CourseLevel] ,[DateAwarded],[Sector],[CreateDay]);
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_StandardCertificateSearchView_Masks
ON [dbo].[StandardCertificateSearchView] ([Uln],[CourseCode],[Ukprn],[Sector]);
GO

CREATE NONCLUSTERED INDEX IX_StandardCertificateSearchView_Masks_Uln
ON [dbo].[StandardCertificateSearchView] ([CourseCode],[Sector],[Ukprn],[CreateDay],[CertificateReferenceId],[Uln])
INCLUDE ([CourseName],[CourseLevel],[ProviderName]);
GO
