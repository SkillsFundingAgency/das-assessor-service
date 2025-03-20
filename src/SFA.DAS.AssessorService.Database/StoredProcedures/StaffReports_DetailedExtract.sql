CREATE PROCEDURE [dbo].[StaffReports_DetailedExtract]
AS
	-- report period is from the start to the end of the previous calendar month at midnight
	DECLARE @FromTime DATETIME = DATEADD(day, 1, EOMONTH(DATEADD(month, -2, GETDATE())))
	DECLARE @ToTime DATETIME = DATEADD(second, -1, DATEADD(day, 1, CAST(EOMONTH(@FromTime) AS DATETIME)))

	SELECT 
		CAST(@FromTime AS DATE) [Month], 
		[StandardCertificates].[Uln] [Apprentice ULN],
		UPPER(JSON_VALUE([StandardCertificates].[CertificateData], '$.FullName')) [Apprentice Names],
		CAST(JSON_VALUE([StandardCertificates].[CertificateData], '$.AchievementDate') AS DATE) [Achievement Date],
		TRIM(UPPER(JSON_VALUE([StandardCertificates].[CertificateData], '$.StandardName'))) [Standard Name],
		[StandardCertificates].[StandardCode] [Standard Code],
		UPPER(JSON_VALUE([StandardCertificates].[CertificateData], '$.StandardReference')) [Standard Reference],
		ISNULL(JSON_VALUE([StandardCertificates].[CertificateData], '$.Version'),'') [Standard Version],
		[Organisations].[EndPointAssessorOrganisationId] [EPAO ID],
		[Organisations].[EndPointAssessorName] [EPAO Name],
		[StandardCertificates].[ProviderUkPrn] [Provider UkPrn],
		UPPER(JSON_VALUE([StandardCertificates].[CertificateData], '$.ProviderName')) [Provider Name],
		CASE
			WHEN [LatestSubmittedPassesBetweenReportDates].[EventTime] IS NULL THEN [StandardCertificates].[Status]
			ELSE [LatestSubmittedPassesBetweenReportDates].[Status]
			END AS [Status]
	FROM
		[StandardCertificates] INNER JOIN [Organisations]
		ON [StandardCertificates].OrganisationId = [Organisations].Id INNER JOIN
		(
			-- take the latest certificate submission from the logs within the report period which is not a fail
			SELECT
				[Action],
				[CertificateId],
				[EventTime],
				[Status]
			FROM
			(
				SELECT 
					[Action],
					[CertificateId],
					[EventTime],
					[Status],
					ROW_NUMBER() OVER (PARTITION BY [CertificateId], [Action] ORDER BY [EventTime]) RowNumber
				FROM [dbo].[CertificateLogs]
				WHERE 
					[Action] = 'Submit'
					AND [EventTime] BETWEEN @FromTime AND @ToTIME
					AND ISNULL(JSON_VALUE([CertificateData],'$.EpaDetails.LatestEpaOutcome'),'Pass') != 'Fail'
			) [SubmittedPassesBetweenReportDates]
			WHERE 
				[SubmittedPassesBetweenReportDates].RowNumber = 1
		) [LatestSubmittedPassesBetweenReportDates] 
		ON [LatestSubmittedPassesBetweenReportDates].[CertificateId] = [StandardCertificates].[Id] AND [StandardCertificates].[CertificateReferenceId] >= 10000 AND [StandardCertificates].[CreatedBy] <> 'manual'
	WHERE
		-- any later certificate submission has not been rescinded
		ISNULL(JSON_VALUE([StandardCertificates].[CertificateData],'$.EpaDetails.LatestEpaOutcome'),'Pass') != 'Fail'
	ORDER BY
		[Month], [Status], [Provider Name], Uln, [Apprentice Names]
RETURN 0