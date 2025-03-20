CREATE PROCEDURE [dbo].[StaffReports_ByEpaoAndStandardAndGrade]
AS
SELECT 
[EPAO Name],
[EPAO ID],
[Standard Name], 
[Standard Code], 
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
	  UPPER(ISNULL([StandardName],'')) AS [Standard Name],
	  ISNULL([StandardReference],'') AS [Standard Reference],
	  ce.[StandardCode] AS [Standard Code],
	  CASE WHEN CONVERT(VARCHAR(10), ce.[StandardLevel]) = '0' THEN '' ELSE 'LEVEL ' + CONVERT(VARCHAR(10), ce.[StandardLevel]) END AS [Level],
	  ISNULL(ce.[Version],'') AS [Standard Version],
	  ISNULL([dbo].[ExpandedVersion](ce.[Version]),'')  AS [orderVersion],	  
	  UPPER(ISNULL(ce.[OverallGrade],'')) AS [Grade],
	  COUNT(*) AS [Total],
	  SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS [Manual Total],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS [EPA Service Total],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS [EPA Service],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END) AS [EPA Service (Manual)],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS [EPA Draft],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS [EPA Submitted],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] IN ('Printed','Delivered','NotDelivered','SentToPrinter','Reprint') THEN 1 ELSE 0 END) AS [EPA Printed],
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS [Deleted],
	  MIN(UPPER(ISNULL([StandardName],''))) OVER (PARTITION BY ISNULL([StandardReference],'')) [sequenceForOrderBy]

  FROM [dbo].[StandardCertificates] ce
  JOIN [dbo].[Organisations] org ON ce.[OrganisationId] = org.[Id]
  WHERE org.EndPointAssessorOrganisationId != 'EPA0000'
  GROUP BY 
	  org.[EndPointAssessorName],
	  org.[EndPointAssessorOrganisationId],
	  UPPER(ISNULL([StandardName],'')),
	  ce.[StandardReference],
	  ce.[StandardCode],
	  CONVERT(VARCHAR(10), ce.[StandardLevel]),
	  ce.[Version],
	  UPPER(ISNULL(ce.[OverallGrade],''))

  UNION
  
  SELECT
	  ' Summary' AS [EPAO Name],
	  '' AS [EPAO ID],
	  '' AS [Standard Name],
	  '' AS [Standard Code],
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
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS [Deleted],
	  null sequenceForOrderBy
  FROM [dbo].[StandardCertificates] ce
  JOIN [dbo].[Organisations] org ON ce.[OrganisationId] = org.[Id]
  WHERE org.EndPointAssessorOrganisationId != 'EPA0000'
) st

  ORDER BY [EPAO ID], [sequenceForOrderBy], [Grade], [orderVersion]
RETURN 0

		 