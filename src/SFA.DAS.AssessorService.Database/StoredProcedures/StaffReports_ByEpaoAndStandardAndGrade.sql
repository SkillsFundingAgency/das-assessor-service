CREATE PROCEDURE [dbo].[StaffReports_ByEpaoAndStandardAndGrade]
AS
SELECT 
[EPAO Name],
[EPAO ID],
[Standard Name], 
[Standard Reference], 
[Level],
[Standard Version],
[Grade],
[Total], 
[Manual Total], 
[EPA Service Total], 
[EPA Service], 
[EPA Service (Manual)], 
[EPA Draft], 
[EPA Submitted], 
[EPA Printed], 
[Deleted]
FROM (
  SELECT
	  org.[EndPointAssessorName] AS [EPAO Name],
	  org.[EndPointAssessorOrganisationId] AS [EPAO ID],
	  TRIM(REPLACE(UPPER(REPLACE(JSON_VALUE(ce.[CertificateData], '$.StandardName'), NCHAR(0x00A0), ' ')), 'Á', ' ')) AS [Standard Name],
	  UPPER(JSON_VALUE(ce.[CertificateData], '$.StandardReference')) AS [Standard Reference],
	  'LEVEL ' + JSON_VALUE(ce.[CertificateData], '$.StandardLevel') AS [Level],
	  ISNULL(JSON_VALUE(ce.[CertificateData], '$.Version'),'') AS [Standard Version],
	  [dbo].[ExpandedVersion](ISNULL(JSON_VALUE(ce.CertificateData, '$.Version'),''))  AS [orderVersion],      
	  (CASE WHEN JSON_VALUE(ce.[CertificateData], '$.OverallGrade') IS NULL THEN '' ELSE  UPPER(JSON_VALUE(ce.[CertificateData], '$.OverallGrade')) END) AS [Grade],
	  COUNT(*) AS [Total],
	  SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS [Manual Total],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS [EPA Service Total],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS [EPA Service],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END) AS [EPA Service (Manual)],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS [EPA Draft],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS [EPA Submitted],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS [EPA Printed],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS [Deleted]
  FROM [dbo].[Certificates] ce
  INNER JOIN [dbo].[Organisations] org ON ce.[OrganisationId] = org.[Id]
  GROUP BY 
	  org.[EndPointAssessorName],
	  org.[EndPointAssessorOrganisationId],
	  TRIM(REPLACE(UPPER(REPLACE(JSON_VALUE(ce.[CertificateData], '$.StandardName'), NCHAR(0x00A0), ' ')), 'Á', ' ')),
	  UPPER(JSON_VALUE(ce.[CertificateData], '$.StandardReference')),
	  'LEVEL ' + JSON_VALUE(ce.[CertificateData], '$.StandardLevel'),
	  JSON_VALUE(ce.[CertificateData], '$.Version'),
	  (CASE WHEN JSON_VALUE(ce.[CertificateData], '$.OverallGrade') IS NULL THEN '' ELSE  UPPER(JSON_VALUE(ce.[CertificateData], '$.OverallGrade')) END)

  UNION
  
  SELECT
	  ' Summary' AS [EPAO Name],
	  '' AS [EPAO ID],
	  '' AS [Standard Name],
	  '' AS [Standard Reference],
	  '' AS [Level],
	  '' AS [Standard Version],
	  '' AS [orderVersion],      
	  '' AS [Grade],
	  COUNT(*) AS [Total], 
	  SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS [Manual Total],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS [EPA Service Total],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS [EPA Service],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END) AS [EPA Service (Manual)],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS [EPA Draft],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS [EPA Submitted],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS [EPA Printed],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS [Deleted]
  FROM [dbo].[Certificates] ce
) st

  ORDER BY [EPAO ID], MIN([Standard Name]) OVER (PARTITION BY [Standard Reference]) /*[Standard Name] */, 
		 [Grade], [orderVersion], 
		 SUM([EPA Service Total]) OVER (PARTITION BY [Standard Reference]) /* [EPA Service Total] */ DESC,
		 SUM([Total]) OVER (PARTITION BY [Standard Reference]) /* [Total] */ DESC
RETURN 0

         