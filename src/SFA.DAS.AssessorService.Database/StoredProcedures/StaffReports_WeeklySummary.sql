CREATE PROCEDURE [dbo].[StaffReports_WeeklySummary]
AS
	SELECT 
		CAST(DATEADD(wk, DATEDIFF(wk, 0, DATEADD(day, -1, [FirstSubmitOrPrintedPassEvent].[EventTime])), 4) AS DATE) ToDate, 
		SUM(CASE WHEN [FirstSubmitOrPrintedPassEvent].[Action] = 'Submit' THEN 1 ELSE 0 END) Submitted,
		SUM(CASE WHEN [FirstSubmitOrPrintedPassEvent].[Action] = 'Submit' AND [Certificates].[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) EPASubmitted,
		SUM(CASE WHEN [FirstSubmitOrPrintedPassEvent].[Action] = 'Submit' AND [Certificates].[CreatedBy] = 'manual' THEN 1 ELSE 0 END) ManualSubmitted,
		SUM(CASE WHEN [FirstSubmitOrPrintedPassEvent].[Action] = 'Printed' THEN 1 ELSE 0 END) Printed,
		SUM(CASE WHEN [FirstSubmitOrPrintedPassEvent].[Action] = 'Printed' AND [Certificates].[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) EPAPrinted,
		SUM(CASE WHEN [FirstSubmitOrPrintedPassEvent].[Action] = 'Printed' AND [Certificates].[CreatedBy] = 'manual' THEN 1 ELSE 0 END) ManualPrinted,
		SUM(CASE WHEN [FirstSubmitOrPrintedPassEvent].[Action] = 'Reprint' THEN 1 ELSE 0 END) Reprint
	INTO
		#WeeklySummary
	FROM 
	(
		SELECT [Action], [CertificateId], [EventTime]
		FROM
		(
			SELECT 
				[Action], [CertificateId], [EventTime], 
				ROW_NUMBER() OVER (PARTITION BY [CertificateId], [Action] ORDER BY [EventTime]) RowNumber
			FROM 
				[dbo].[CertificateLogs]
			WHERE 
				[Action] IN ('Submit', 'Printed', 'Reprint')
				AND ISNULL(LatestEpaOutcome,'Pass') != 'Fail'
		) [SubmitOrPrintedPassEvents]
		WHERE [SubmitOrPrintedPassEvents].RowNumber = 1
	) 
	[FirstSubmitOrPrintedPassEvent] JOIN [dbo].[Certificates] [Certificates] 
		ON [Certificates].[Id] = [FirstSubmitOrPrintedPassEvent].[CertificateId]
	GROUP BY 
		-- the week ending date (Friday)
		CAST(DATEADD(wk, DATEDIFF(wk, 0, DATEADD(day, -1, [FirstSubmitOrPrintedPassEvent].[EventTime])), 4) AS DATE)

	SELECT
		CAST(ToDate AS VARCHAR) 'To Date', 
		Submitted,
		EPASubmitted 'EPA Submitted',
		ManualSubmitted 'Manual Submitted',
		Printed,
		EPAPrinted 'EPA Printed',
		ManualPrinted 'Manual Printed',
		Reprint
	FROM
		#WeeklySummary

	UNION

	SELECT
		' Summary' 'To Date', 
		SUM(Submitted) 'Submitted',
		SUM(EPASubmitted) 'EPA Submitted',
		SUM(ManualSubmitted) 'Manual Submitted',
		SUM(Printed) 'Printed',
		SUM(EPAPrinted) 'EPA Printed',
		SUM(ManualPrinted) 'Manual Printed',
		SUM(Reprint) 'Reprint'
	FROM
		#WeeklySummary

	ORDER BY 
		'To Date' 
RETURN 0
