CREATE PROCEDURE [DashboardReporting].[CertificatePrintSummary]
	@from DATETIME,
	@to DATETIME
AS
	BEGIN TRY
		IF(DATEDIFF(day, @from, @to) > 28)
		BEGIN
			DECLARE @DateRangeErrorMessage NVARCHAR(100) = 'Date range from: ' + FORMAT(@from, 'g', 'en-gb') + ' to: ' + FORMAT(@to, 'g', 'en-gb') + ' is greater than maximum date range of 28 days'
			RAISERROR(@DateRangeErrorMessage, 16, 1)
		END

		SELECT 
			[PivotPrintCertificateLogsByBatchNumber].BatchNumber,
			FORMAT([BatchLogs].ScheduledDate, 'g', 'en-gb' ) ScheduledDate,
			FORMAT([BatchLogs].FileUploadEndTime, 'g', 'en-gb' ) UploadedDate,
			FORMAT(PARSE(JSON_VALUE([BatchLogs].BatchData, '$.PrintedDate') AS DATETIME), 'g', 'en-gb') PrintedDate,
			[PivotPrintCertificateLogsByBatchNumber].SentToPrinter, 
			[PivotPrintCertificateLogsByBatchNumber].Printed, 
			[PivotPrintCertificateLogsByBatchNumber].Delivered, 
			[PivotPrintCertificateLogsByBatchNumber].NotDelivered
		FROM
		(
			SELECT 
				[PivotPrintCertificateLogs].BatchNumber,
				SUM([SentToPrinter]) SentToPrinter, SUM([Printed]) Printed, SUM([Delivered]) Delivered, SUM([NotDelivered]) NotDelivered
			FROM  
			(
				SELECT 
					Status, BatchNumber, EventTime FROM [CertificateLogs] 
				WHERE 
					EventTime >= @from AND EventTime <= @to
					AND 
					((Action = 'Status' AND Status = 'SentToPrinter')
						OR (Action = 'Printed' AND Status = 'Printed')
						OR (Action = 'Status' AND Status = 'Delivered')
						OR (Action = 'Status' AND Status = 'NotDelivered'))
					AND BatchNumber IS NOT NULL
			) AS [PrintCertificateLogs]
			PIVOT  
			(  
				COUNT(Status)
				FOR Status IN ([SentToPrinter], [Printed], [Delivered], [NotDelivered])
			) AS [PivotPrintCertificateLogs]
			GROUP BY 
				BatchNumber
		) AS [PivotPrintCertificateLogsByBatchNumber]
		JOIN BatchLogs 
			ON [PivotPrintCertificateLogsByBatchNumber].BatchNumber = [BatchLogs].BatchNumber
		ORDER BY [PivotPrintCertificateLogsByBatchNumber].BatchNumber
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();  
  
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);  
	END CATCH
RETURN 0
