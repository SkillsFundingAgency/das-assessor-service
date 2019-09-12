CREATE PROCEDURE [dbo].[OppFinder_Approved_Standards]
	 @SortColumn AS NVARCHAR(20),
	 @SortAscending AS INT,
     @PageSize AS INT,
     @PageIndex AS INT,
     @TotalCount AS INT OUTPUT
AS
BEGIN
	-- redeclare variables to workaround query plan caching performance issues
	DECLARE @SortColumnInternal NVARCHAR(20) = @SortColumn
	DECLARE @SortAscendingInternal INT = @SortAscending
	DECLARE @PageSizeInternal INT = @PageSize
	DECLARE @PageIndexInternal INT = @PageIndex

	DECLARE @Skip INT = (@PageIndexInternal-1) * @PageSizeInternal

	SELECT 
		StandardCode, 
		StandardReference, 
		StandardName, 
		SUM(Learners) ActiveApprentices, 
		MAX(TotalEPAOs) RegisteredEPAOs,
		TotalCount = COUNT(1) OVER()
	INTO
		#Results
	FROM 
		StandardSummary
	GROUP BY 
		StandardCode, StandardReference, StandardName
	ORDER BY
		CASE WHEN @SortAscendingInternal = 0 THEN ''
		ELSE
           CASE 
				WHEN @SortColumnInternal = 'StandardName' THEN StandardName
				-- all dynamic order by columns must the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @SortColumnInternal = 'ActiveApprentices' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(SUM(Learners) AS VARCHAR(MAX)))), 6)
				WHEN @SortColumnInternal = 'RegisteredEPAOs' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(MAX(TotalEPAOs) AS VARCHAR(MAX)))), 6)
				ELSE RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(StandardCode AS VARCHAR(MAX)))), 6)
			END
		END ASC,
		CASE WHEN @SortAscendingInternal = 1 THEN ''
		ELSE
           CASE 
				WHEN @SortColumnInternal = 'StandardName' THEN StandardName
				-- all dynamic order by columns must the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @SortColumnInternal = 'ActiveApprentices' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(SUM(Learners) AS VARCHAR(MAX)))), 6)
				WHEN @SortColumnInternal = 'RegisteredEPAOs' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(MAX(TotalEPAOs) AS VARCHAR(MAX)))), 6)
				ELSE RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(StandardCode AS VARCHAR(MAX)))), 6)
			END
		END DESC
	OFFSET @Skip ROWS
	FETCH NEXT @PageSizeInternal ROWS ONLY

	SELECT @TotalCount = (SELECT MAX(TotalCount) FROM #Results)

	SELECT 
		StandardCode, 
		StandardReference, 
		StandardName, 
		ActiveApprentices, 
		RegisteredEPAOs
	FROM 
		#Results
	ORDER BY -- ensure that results are ordered on final select from temporary table
		CASE WHEN @SortAscendingInternal = 0 THEN ''
		ELSE
           CASE 
				WHEN @SortColumnInternal = 'StandardName' THEN StandardName
				-- all dynamic order by columns must the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @SortColumnInternal = 'ActiveApprentices' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(ActiveApprentices AS VARCHAR(MAX)))), 6)
				WHEN @SortColumnInternal = 'RegisteredEPAOs' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(RegisteredEPAOs AS VARCHAR(MAX)))), 6)
				ELSE RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(StandardCode AS VARCHAR(MAX)))), 6)
			END
		END ASC,
		CASE WHEN @SortAscendingInternal = 1 THEN ''
		ELSE
           CASE 
				WHEN @SortColumnInternal = 'StandardName' THEN StandardName
				-- all dynamic order by columns must the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @SortColumnInternal = 'ActiveApprentices' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(ActiveApprentices AS VARCHAR(MAX)))), 6)
				WHEN @SortColumnInternal = 'RegisteredEPAOs' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(RegisteredEPAOs AS VARCHAR(MAX)))), 6)
				ELSE RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(StandardCode AS VARCHAR(MAX)))), 6)
			END
		END DESC
END
