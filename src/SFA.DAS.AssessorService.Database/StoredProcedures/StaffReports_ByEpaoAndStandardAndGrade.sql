CREATE PROCEDURE [dbo].[StaffReports_ByEpaoAndStandardAndGrade]
AS
  SELECT
	  org.[EndPointAssessorName] AS 'EPAO Name',
	  org.[EndPointAssessorOrganisationId] AS 'EPAO ID',
	  REPLACE(UPPER(REPLACE(JSON_VALUE(ce.[CertificateData], '$.StandardName'), NCHAR(0x00A0), ' ')), 'Á', ' ') AS 'Standard Name',
	  CONVERT(CHAR(10), ce.[StandardCode]) AS 'Standard Code',
	  'LEVEL ' + JSON_VALUE(ce.[CertificateData], '$.StandardLevel') AS 'Level',
	  (CASE WHEN JSON_VALUE(ce.[CertificateData], '$.OverallGrade') IS NULL THEN '' ELSE  UPPER(JSON_VALUE(ce.[CertificateData], '$.OverallGrade')) END) AS 'Grade',
	  COUNT(*) AS 'Total', 
	  SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS 'Manual Total',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS 'EPA Service Total',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[createdby] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Service',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[createdby] = 'manual' THEN 1 ELSE 0 END) AS 'EPA Service (Manual)',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[status] = 'Draft' THEN 1 ELSE 0 END) AS 'EPA Draft',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[status] = 'Submitted' THEN 1 ELSE 0 END) AS 'EPA Submitted',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[status] = 'Printed' THEN 1 ELSE 0 END) AS 'EPA Printed',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted'
  FROM [dbo].[Certificates] ce
  INNER JOIN [dbo].[Organisations] org ON ce.[OrganisationId] = org.[Id]
  GROUP BY 
	  org.[EndPointAssessorName],
	  org.[EndPointAssessorOrganisationId],
	  REPLACE(UPPER(REPLACE(JSON_VALUE(ce.[CertificateData], '$.StandardName'), NCHAR(0x00A0), ' ')), 'Á', ' '),
	  ce.[StandardCode],
	  'LEVEL ' + JSON_VALUE(ce.[CertificateData], '$.StandardLevel'),
	  (CASE WHEN JSON_VALUE(ce.[CertificateData], '$.OverallGrade') IS NULL THEN '' ELSE  UPPER(JSON_VALUE(ce.[CertificateData], '$.OverallGrade')) END)

  UNION
  
  SELECT
	  ' Summary' AS 'EPAO Name',
	  '' AS 'EPAO ID',
	  '' AS 'Standard Name',
	  '' 'Standard Code',
	  '' AS 'Level',
	  '' AS 'Grade',
	  COUNT(*) AS 'Total', 
	  SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS 'Manual Total',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS 'EPA Service Total',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[createdby] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Service',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[createdby] = 'manual' THEN 1 ELSE 0 END) AS 'EPA Service (Manual)',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[status] = 'Draft' THEN 1 ELSE 0 END) AS 'EPA Draft',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[status] = 'Submitted' THEN 1 ELSE 0 END) AS 'EPA Submitted',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[status] = 'Printed' THEN 1 ELSE 0 END) AS 'EPA Printed',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted'
  FROM [dbo].[Certificates] ce

  ORDER BY 2, 4, 6, 10 DESC, 9 DESC, 7 DESC
RETURN 0
