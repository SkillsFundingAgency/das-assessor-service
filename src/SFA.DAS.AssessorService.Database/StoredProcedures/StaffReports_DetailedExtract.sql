CREATE PROCEDURE [dbo].[StaffReports_DetailedExtract]
       @fromdate NVARCHAR(16),
       @todate NVARCHAR(16)
AS
	SELECT
		   CONVERT(VARCHAR(10), DATEADD(mm, DATEDIFF(mm, 0, DATEADD(mm, 0, cl.[EventTime])), 0), 120) AS 'Month',
		   ce.[Uln] AS 'Apprentice ULN',
		   UPPER(JSON_VALUE(ce.[CertificateData], '$.FullName')) AS 'Apprentice Names',
		   CONVERT(CHAR(10), JSON_VALUE(ce.[CertificateData], '$.AchievementDate')) AS 'Achievement Date',
		   UPPER(JSON_VALUE(ce.[CertificateData], '$.StandardName')) AS 'Standard Name',
		   ce.[StandardCode] AS 'Standard Code',
		   rg.[EndPointAssessorOrganisationId] AS 'EPAO ID',
		   rg.[EndPointAssessorName] AS 'EPAO Name',
		   ce.[ProviderUkPrn] AS 'Provider UkPrn',
		   UPPER(JSON_VALUE(ce.[CertificateData], '$.ProviderName')) AS 'Provider Name',
		   CASE
			   WHEN cl.[EventTime] IS NULL THEN ce.[Status]
			   ELSE cl.[Status]
		   END AS 'Status'
	FROM [dbo].[Certificates] ce
	JOIN [dbo].[Organisations] rg ON ce.[OrganisationId] = rg.[Id]
	JOIN
	  (SELECT [Action],
			  [CertificateId],
			  [EventTime],
			  [Status]
	   FROM
		 (SELECT [Action],
				 [CertificateId],
				 [EventTime],
				 [Status],
				 ROW_NUMBER() OVER (PARTITION BY [CertificateId], [Action] ORDER BY [EventTime]) rownumber
		  FROM [dbo].[CertificateLogs]
		  WHERE ACTION IN ('Submit') AND [EventTime] >= @fromdate AND [EventTime] < @todate) ab
	   WHERE ab.rownumber = 1 ) cl ON cl.[CertificateId] = ce.[Id] AND ce.[CertificateReferenceId] >= 10000 AND ce.[CreatedBy] <> 'manual'
	WHERE ISNULL(JSON_VALUE(ce.[CertificateData],'$.EpaDetails.LatestEpaOutcome'),'Pass') != 'Fail'
	UNION
	SELECT CONVERT(VARCHAR(10), DATEADD(mm, DATEDIFF(mm, 0, DATEADD(mm, 0, ce.[CreatedAt])), 0), 120) AS 'Month',
		   ce.[Uln] AS 'Apprentice ULN',
		   UPPER(JSON_VALUE(ce.[CertificateData], '$.FullName')) AS 'Apprentice Names',
		   CONVERT(CHAR(10), JSON_VALUE(ce.[CertificateData], '$.AchievementDate')) AS 'Achievement Date',
		   UPPER(JSON_VALUE(ce.[CertificateData], '$.StandardName')) AS 'Standard Name',
		   ce.[StandardCode] AS 'Standard Code',
		   rg.[EndPointAssessorOrganisationId] AS 'EPAO ID',
		   rg.[EndPointAssessorName] AS 'EPAO Name',
		   ce.[ProviderUkPrn]  AS 'Provider UkPrn',
		   UPPER(JSON_VALUE(ce.[CertificateData], '$.ProviderName')) AS 'Provider Name',
		   ce.[Status] AS 'Status'
	FROM [dbo].[Certificates] ce
	JOIN [dbo].[Organisations] rg ON ce.[OrganisationId] = rg.[Id]
	WHERE ce.[Status] = 'Draft' AND ce.CreateDay BETWEEN @fromdate AND @todate
	AND JSON_VALUE(ce.[CertificateData], '$.FullName') IS NOT NULL
	AND JSON_VALUE(ce.[CertificateData], '$.StandardName') IS NOT NULL
	AND ISNULL(JSON_VALUE(ce.[CertificateData], '$.ProviderName'),'') != '' 
	ORDER BY 1, 11, 10, 2, 3
RETURN 0