CREATE PROCEDURE [DashboardReporting].[IlrImportSummary]
	@from DATETIME,
	@to DATETIME
AS
	BEGIN TRY
		IF(DATEDIFF(day, @from, @to) > 28)
		BEGIN
			DECLARE @DateRangeErrorMessage NVARCHAR(100) = 'Date range from: ' + FORMAT(@from, 'g', 'en-gb') + ' to: ' + FORMAT(@to, 'g', 'en-gb') + ' is greater than maximum date range of 28 days'
			RAISERROR(@DateRangeErrorMessage, 16, 1)
		END

		SELECT FORMAT( COALESCE(CAST ([UpdatedAt] AS DATE), CAST ([CreatedAt] AS DATE) ) , 'g', 'en-gb' ) AS IlrDate,
			  COUNT(*) AS IlrCount
		FROM  [dbo].[Ilrs]
		WHERE COALESCE(CAST ([UpdatedAt] AS DATE), CAST ([CreatedAt] AS DATE))  BETWEEN  @from AND @to
		GROUP BY COALESCE(CAST ([UpdatedAt] AS DATE), CAST ([CreatedAt] AS DATE)) 
		ORDER BY COALESCE(CAST ([UpdatedAt] AS DATE), CAST ([CreatedAt] AS DATE))

	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();  
  
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);  
	END CATCH
RETURN 0
