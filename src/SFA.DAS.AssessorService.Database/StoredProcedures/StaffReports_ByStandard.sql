CREATE PROCEDURE [dbo].[StaffReports_ByStandard]
AS
  SELECT
		REPLACE(UPPER(REPLACE(JSON_VALUE(ce.[CertificateData], '$.StandardName'), NCHAR(0x00A0), ' ')), 'Á', ' ') AS 'Standard Name',
		JSON_VALUE(ce.CertificateData, '$.StandardReference') 'Standard Reference',
		JSON_VALUE(ce.CertificateData, '$.Version') 'Standard Version',
		COUNT(*) AS 'Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS 'Manual Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS 'EPA Service Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Service',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END ) AS 'EPA Service Manual',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS 'EPA Draft',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS 'EPA Submitted',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS 'EPA Printed',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted'
  FROM [dbo].[Certificates] ce
  GROUP BY REPLACE(UPPER(REPLACE(JSON_VALUE(ce.[CertificateData], '$.StandardName'), NCHAR(0x00A0), ' ')), 'Á', ' '), ce.[StandardCode],  ce.CertificateData

  UNION 
  SELECT
		' Summary' AS 'Standard Name',
		''  AS 'Standard Reference',
		''  AS 'Standard Version',
		COUNT(*) AS 'Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS 'Manual Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS 'EPA Service Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Service',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END ) AS 'EPA Service Manual',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS 'EPA Draft',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS 'EPA Submitted',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS 'EPA Printed',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted'
  FROM [dbo].[Certificates] ce

  ORDER BY 6 DESC, 5 DESC, 3 DESC, 1
RETURN 0