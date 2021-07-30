CREATE PROCEDURE [dbo].[StaffReports_MissingCertificateData]
AS
	SELECT 
		CertificateReference 'Certificate Reference',
		o.EndPointAssessorName EPAO,
		ISNULL(JSON_VALUE(CertificateData, '$.LearnerGivenNames'), '') + ' ' + ISNULL(JSON_VALUE(CertificateData, '$.LearnerFamilyName'), '') 'Apprentice Name',
		c.Status,
		ISNULL(JSON_VALUE(CertificateData, '$.ContactAddLine1'), '(required)') 'Building and street 1',
		ISNULL(JSON_VALUE(CertificateData, '$.ContactAddLine2'), '') 'Building and street 2',
		ISNULL(JSON_VALUE(CertificateData, '$.ContactAddLine3'), '') 'Building and street 3',
		ISNULL(JSON_VALUE(CertificateData, '$.ContactPostCode'), '(required)') Postcode
	FROM 
		[Certificates] c INNER JOIN [Organisations] o
			ON c.OrganisationId = o.Id
	WHERE 
		(JSON_VALUE(CertificateData, '$.ContactAddLine1') IS NULL OR JSON_VALUE(CertificateData, '$.ContactPostCode') IS NULL) 
		AND c.Status <> 'Draft' AND JSON_VALUE(CertificateData, '$.EpaDetails.LatestEpaOutcome') <> 'Fail'
	ORDER BY 
		ISNULL(c.UpdatedAt, c.CreatedAt) DESC

	RETURN 0
