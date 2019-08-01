CREATE PROCEDURE [dbo].[StaffReports_MonthlySummary]
AS
	SELECT 
		CONVERT(VARCHAR(10), DATEADD(mm, DATEDIFF(mm, 0, DATEADD(mm, 0, cl.[EventTime])), 0), 120) AS 'To Date', 
		SUM(CASE WHEN cl.[Action] = 'Submit' THEN 1 ELSE 0 END) AS 'Submitted',
		SUM(CASE WHEN cl.[Action] = 'Submit' AND ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Submitted',
		SUM(CASE WHEN cl.[Action] = 'Submit' AND ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END) AS 'Manual Submitted',
		SUM(CASE WHEN cl.[Action] = 'Printed' THEN 1 ELSE 0 END) AS 'Printed',
		SUM(CASE WHEN cl.[Action] = 'Printed' AND ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Printed',
		SUM(CASE WHEN cl.[Action] = 'Printed' AND ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END) AS 'Manual Printed',
		SUM(CASE WHEN cl.[Action] = 'Reprint' THEN 1 ELSE 0 END) AS 'Reprint'
	FROM 
	(
		SELECT [Action], [CertificateId], [EventTime]
		FROM
		(
			SELECT [Action], [CertificateId], [EventTime], row_number() OVER (partition by [CertificateId], [Action] ORDER BY [EventTime]) rownumber
			FROM [dbo].[CertificateLogs]
			WHERE [Action] IN ('submit', 'printed') 
			AND ISNULL(JSON_VALUE([CertificateData],'$.EpaDetails.LatestEpaOutcome'),'Pass') != 'Fail'
		) ab 
		WHERE ab.rownumber = 1
	) cl
	JOIN [dbo].[Certificates] ce ON ce.[Id] = cl.[CertificateId]
	WHERE cl.[Action] IN ('Submit', 'Printed', 'Reprint')
	AND ISNULL(JSON_VALUE(ce.[CertificateData],'$.EpaDetails.LatestEpaOutcome'),'Pass') != 'Fail'
	GROUP BY CONVERT(VARCHAR(10), DATEADD(mm, DATEDIFF(mm, 0, DATEADD(mm, 0, cl.[EventTime])), 0), 120)

	UNION

	SELECT
		' Summary' AS 'To Date', 
		SUM(CASE WHEN cl.[Action] = 'Submit' THEN 1 ELSE 0 END) AS 'Submitted',
		SUM(CASE WHEN cl.[Action] = 'Submit' AND ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Submitted',
		SUM(CASE WHEN cl.[Action] = 'Submit' AND ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END) AS 'Manual Submitted',
		SUM(CASE WHEN cl.[Action] = 'Printed' THEN 1 ELSE 0 END) AS 'Printed',
		SUM(CASE WHEN cl.[Action] = 'Printed' AND ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Printed',
		SUM(CASE WHEN cl.[Action] = 'Printed' AND ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END) AS 'Manual Printed',
		SUM(CASE WHEN cl.[Action] = 'Reprint' THEN 1 ELSE 0 END) AS 'Reprint'
	FROM 
	(
		SELECT [Action], [CertificateId], [EventTime]
		FROM
		(
			SELECT [Action], [CertificateId], [EventTime], row_number() OVER (partition by [CertificateId], [Action] ORDER BY [EventTime]) rownumber
			FROM [dbo].[CertificateLogs]
			WHERE [Action] IN ('submit', 'printed') 
			AND ISNULL(JSON_VALUE([CertificateData],'$.EpaDetails.LatestEpaOutcome'),'Pass') != 'Fail'
		) ab 
		WHERE ab.rownumber = 1
	) cl
	JOIN [dbo].[Certificates] ce ON ce.[Id] = cl.[CertificateId]
	WHERE cl.[Action] IN ('Submit', 'Printed', 'Reprint')
	AND ISNULL(JSON_VALUE(ce.[CertificateData],'$.EpaDetails.LatestEpaOutcome'),'Pass') != 'Fail'
	ORDER BY 1 
RETURN 0
