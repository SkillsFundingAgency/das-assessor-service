CREATE VIEW [DashboardReporting].[CertificatePrintSummaryLast28Days]
AS
	SELECT 
		[PivotPrintCertificateLogsInLast28Days].BatchNumber,
		FORMAT([BatchLogs].ScheduledDate, 'g', 'en-gb' ) ScheduledDate,
		FORMAT([BatchLogs].FileUploadEndTime, 'g', 'en-gb' ) UploadedDate,
		FORMAT(PARSE(JSON_VALUE([BatchLogs].BatchData, '$.PrintedDate') AS DATETIME), 'g', 'en-gb') PrintedDate,
		[SentToPrinter], [Printed], [Delivered], [NotDelivered]
	FROM  
	(
		SELECT 
			Status, BatchNumber FROM [CertificateLogs] 
		WHERE 
			EventTime > DATEADD(day, -28, GETDATE()) 
			AND ((Action = 'Status' AND Status = 'SentToPrinter')
				 OR (Action = 'Printed' AND Status = 'Printed')
				 OR (Action = 'Status' AND Status = 'Delivered')
				 OR (Action = 'Status' AND Status = 'NotDelivered'))
			AND BatchNumber IS NOT NULL
	) AS [PrintCertificateLogsInLast28Days]
	PIVOT  
	(  
	  COUNT(Status)
	  FOR Status IN ([SentToPrinter], [Printed], [Delivered], [NotDelivered])
	) AS [PivotPrintCertificateLogsInLast28Days]
	JOIN BatchLogs 
		ON [PivotPrintCertificateLogsInLast28Days].BatchNumber = [BatchLogs].BatchNumber
