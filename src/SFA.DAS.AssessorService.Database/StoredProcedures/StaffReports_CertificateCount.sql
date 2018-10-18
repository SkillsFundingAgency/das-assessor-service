CREATE PROCEDURE [dbo].[StaffReports_CertificateCount]
AS
	SELECT 
		CONVERT(CHAR(12), GETDATE(), 103) AS 'To Date', 
		SUM(CASE WHEN ce.[DeletedAt] IS NULL THEN 1 ELSE 0 END) AS 'Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] <> 'Submitted' THEN 1 ELSE 0 END) AS 'Manual Printed',
		SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS 'Manual Submitted',
		(SELECT COUNT(*) FROM [dbo].[CertificateLogs] WHERE [Status] = 'DUPLICATE' AND username = 'Manual') AS 'Duplicated',
		(SELECT COUNT(DISTINCT ce2.[OrganisationId]) FROM [dbo].[Certificates] ce2 WHERE ce2.CreatedBy <> 'Manual') AS 'EPAOs',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS 'EPA Service',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS 'EPA Service Printed',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS 'EPA Service Submitted',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS 'EPA Service Draft',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted',
		SUM(CASE WHEN ce.[DeletedAt] IS NULL AND ce.[Status] NOT IN ('Draft','Deleted') THEN 1 ELSE 0 END) AS 'Completed Certs',
		SUM(CASE WHEN ce.[DeletedAt] IS NULL AND ce.[Status] IN ('Draft')  THEN 1 ELSE 0 END) AS 'Draft Certs',
		SUM(CASE WHEN ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted Certs',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL THEN 1 ELSE 0 END) AS 'EPA Service Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND [CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Service Only',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND [CreatedBy] = 'manual' THEN 1 ELSE 0 END) AS 'EPA Service (Manual)'
	FROM [dbo].[Certificates] ce
RETURN 0
