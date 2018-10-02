CREATE PROCEDURE [dbo].[StaffReports_ByBatch]
AS
	SELECT 
		CONVERT(CHAR(12), GETDATE(), 103) AS 'To Date',
		ce.[BatchNumber] AS 'Batch Number',
		CONVERT(CHAR(16), ce.[ToBePrinted]) 'Printed',
		COUNT(*) AS 'Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS 'Manual',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS 'EPA Service Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[createdby] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Service',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[createdby] = 'manual' THEN 1 ELSE 0 END) AS 'EPA Service (Manual)',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS 'Printed',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS 'Submitted',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS 'Draft',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted'
	FROM [dbo].[Certificates] ce
	GROUP BY ce.[BatchNumber], CONVERT(CHAR(16), ce.[ToBePrinted])
	ORDER BY 3 DESC, 2
RETURN 0
