CREATE PROCEDURE [dbo].[StaffReports_DetailedExtract]
       @fromdate NVARCHAR(16),
       @todate NVARCHAR(16)
AS
	SELECT
		   CONVERT(VARCHAR(10), DATEADD(mm, DATEDIFF(mm, 0, DATEADD(mm, 0, cl.[EventTime])), 0), 120) AS 'Month',
		   ce.[ULN] AS 'Apprentice ULN',
		   UPPER(JSON_VALUE(ce.[CertificateData], '$.FullName')) AS 'Apprentice Names',
		   CONVERT(CHAR(10), JSON_VALUE(ce.[CertificateData], '$.AchievementDate')) AS 'Achievement Date',
		   UPPER(JSON_VALUE(ce.[CertificateData], '$.StandardName')) AS 'Standard Name',
		   ce.[StandardCode] AS 'Standard Code',
		   rg.[EndPointAssessorOrganisationId] AS 'EPAO ID',
		   rg.[EndPointAssessorName] AS 'EPAO Name',
		   ce.[ProviderUkPrn] AS 'Provider UkPrn',
		   CASE ce.[ProviderUkPrn]
			   WHEN 10004344 THEN 'MIDDLESBROUGH COLLEGE'
			   WHEN 10005991 THEN 'CENTRAL COLLEGE NOTTINGHAM'
			   WHEN 10007315 THEN 'WALSALL COLLEGE'
			   WHEN 10007949 THEN 'HUNTINGDONSHIRE REGIONAL COLLEGE'
			   WHEN 10021221 THEN 'MERSEY CARE NHS FOUNDATION TRUST'
			   WHEN 10001548 THEN 'COLLEGE OF HARINGEY, ENFIELD AND NORTH-EAST LONDON, THE'
			   WHEN 10007144 THEN 'UNIVERSITY OF EAST LONDON'
			   WHEN 10005404 THEN 'REASEHEATH COLLEGE'
			   WHEN 10005077 THEN 'PETERBOROUGH REGIONAL COLLEGE'
			   WHEN 10007431 THEN 'WEST SUFFOLK COLLEGE'
			   WHEN 10007697 THEN 'YH TRAINING SERVICES LIMITED'
			   ELSE (SELECT MAX(UPPER(JSON_VALUE([CertificateData], '$.ProviderName'))) FROM [dbo].[Certificates] WHERE [ProviderUkPrn] = ce.[ProviderUkPrn])
		   END AS 'Provider Name',
		   CASE
			   WHEN cl.[EventTime] IS NULL THEN ce.[Status]
			   ELSE cl.[Status]
		   END AS 'Status'
	FROM [dbo].[Certificates] ce
	JOIN [dbo].[Organisations] rg ON ce.[OrganisationId] = rg.[id]
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
		  WHERE ACTION IN ('Submit') AND [EventTime] BETWEEN @fromdate AND @todate) ab
	   WHERE ab.rownumber = 1 ) cl ON cl.[CertificateId] = ce.[Id] AND ce.[CertificateReferenceId] >= 10000 AND ce.[CreatedBy] <> 'manual'
	UNION
	SELECT CONVERT(VARCHAR(10), DATEADD(mm, DATEDIFF(mm, 0, DATEADD(mm, 0, ce.[CreatedAt])), 0), 120) AS 'Month',
		   ce.[ULN] AS 'Apprentice ULN',
		   UPPER(JSON_VALUE(ce.[CertificateData], '$.FullName')) AS 'Apprentice Names',
		   CONVERT(CHAR(10), JSON_VALUE(ce.[CertificateData], '$.AchievementDate')) AS 'Achievement Date',
		   UPPER(JSON_VALUE(ce.[CertificateData], '$.StandardName')) AS 'Standard Name',
		   ce.[StandardCode] AS 'Standard Code',
		   rg.[EndPointAssessorOrganisationId] AS 'EPAO ID',
		   rg.[EndPointAssessorName] AS 'EPAO Name',
		   ce.[ProviderUkPrn]  AS 'Provider UkPrn',
		   CASE ce.[ProviderUkPrn]
			   WHEN 10004344 THEN 'MIDDLESBROUGH COLLEGE'
			   WHEN 10005991 THEN 'CENTRAL COLLEGE NOTTINGHAM'
			   WHEN 10007315 THEN 'WALSALL COLLEGE'
			   WHEN 10007949 THEN 'HUNTINGDONSHIRE REGIONAL COLLEGE'
			   WHEN 10021221 THEN 'MERSEY CARE NHS FOUNDATION TRUST'
			   WHEN 10001548 THEN 'COLLEGE OF HARINGEY, ENFIELD AND NORTH-EAST LONDON, THE'
			   WHEN 10007144 THEN 'UNIVERSITY OF EAST LONDON'
			   WHEN 10005404 THEN 'REASEHEATH COLLEGE'
			   WHEN 10005077 THEN 'PETERBOROUGH REGIONAL COLLEGE'
			   WHEN 10007431 THEN 'WEST SUFFOLK COLLEGE'
			   WHEN 10007697 THEN 'YH TRAINING SERVICES LIMITED'
			   ELSE (SELECT MAX(UPPER(JSON_VALUE([CertificateData], '$.ProviderName'))) FROM [dbo].[Certificates] WHERE [ProviderUkPrn] = ce.[ProviderUkPrn])
		   END AS 'Provider Name',
		   ce.[Status] AS 'Status'
	FROM [dbo].[Certificates] ce
	JOIN [dbo].[Organisations] rg ON ce.[OrganisationId] = rg.[id]
	WHERE ce.[Status] = 'Draft' AND ce.CreatedAt BETWEEN @fromdate AND @todate
	ORDER BY 1, 11, 10, 2, 3
RETURN 0