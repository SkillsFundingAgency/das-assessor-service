CREATE PROCEDURE [dbo].[StaffReports_ByEpao]
AS
  SELECT
	  org.[EndPointAssessorName] AS 'EPAO Name',
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
  GROUP BY org.[EndPointAssessorName]

  UNION 

  SELECT
	  ' Summary' AS 'EPAO Name',
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

  ORDER BY 5 DESC, 4 DESC, 2 DESC, 1
RETURN 0
