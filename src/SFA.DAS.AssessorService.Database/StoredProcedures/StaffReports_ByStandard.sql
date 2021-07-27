CREATE PROCEDURE [dbo].[StaffReports_ByStandard]
AS
SELECT 
[Standard Name], 
[Standard Reference], 
[Standard Version],
[Total], 
[Manual Total], 
[EPA Service Total], 
[EPA Service], 
[EPA Service Manual], 
[EPA Draft], 
[EPA Submitted], 
[EPA Printed], 
[Deleted]
FROM (
  SELECT
        TRIM(REPLACE(UPPER(REPLACE(JSON_VALUE(ce.[CertificateData], '$.StandardName'), NCHAR(0x00A0), ' ')), 'Á', ' ')) AS [Standard Name],
        JSON_VALUE(ce.CertificateData, '$.StandardReference') AS [Standard Reference],
        ISNULL(JSON_VALUE(ce.CertificateData, '$.Version'),'') AS [Standard Version],
		[dbo].[ExpandedVersion](ISNULL(JSON_VALUE(ce.CertificateData, '$.Version'),''))  AS [orderVersion],
        COUNT(*) AS [Total],
        SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS [Manual Total],
        SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS [EPA Service Total],
        SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS [EPA Service],
        SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END ) AS [EPA Service Manual],
        SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS [EPA Draft],
        SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS [EPA Submitted],
        SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS [EPA Printed],
        SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS [Deleted]
  FROM [dbo].[Certificates] ce
  WHERE JSON_VALUE(ce.CertificateData, '$.StandardReference') IS NOT NULL
  GROUP BY TRIM(REPLACE(UPPER(REPLACE(JSON_VALUE(ce.[CertificateData], '$.StandardName'), NCHAR(0x00A0), ' ')), 'Á', ' ')),
        JSON_VALUE(ce.CertificateData, '$.StandardReference'),ISNULL(JSON_VALUE(ce.CertificateData, '$.Version'),'') 

  UNION 
  SELECT
		' Summary' AS [Standard Name],
		''  AS [Standard Reference],
		''  AS [Standard Version],
		''  AS [orderVersion],
		COUNT(*) AS [Total],
		SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS [Manual Total],
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS [EPA Service Total],
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS [EPA Service],
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END ) AS [EPA Service Manual],
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS [EPA Draft],
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS [EPA Submitted],
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS [EPA Printed],
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS [Deleted]
  FROM [dbo].[Certificates] ce
  WHERE JSON_VALUE(ce.CertificateData, '$.StandardReference') IS NOT NULL
) st

ORDER BY SUM([EPA Service]) OVER (PARTITION BY [Standard Reference]) /* [EPA Service]*/ DESC,
         MIN([Standard Name]) OVER (PARTITION BY [Standard Reference]) /*[Standard Name] */, 
		 [orderVersion], 
		 SUM([EPA Service Total]) OVER (PARTITION BY [Standard Reference]) /* [EPA Service Total] */ DESC,
		 SUM([Total]) OVER (PARTITION BY [Standard Reference]) /* [Total] */ DESC
RETURN 0