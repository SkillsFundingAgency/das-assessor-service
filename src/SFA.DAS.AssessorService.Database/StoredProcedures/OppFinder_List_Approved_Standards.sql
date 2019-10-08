CREATE PROCEDURE [dbo].[OppFinder_List_Approved_Standards]
	 @SearchTerm AS NVARCHAR(100),
	 @SectorFilters AS NVARCHAR(MAX),
	 @LevelFilters AS NVARCHAR(MAX),
	 @SortColumn AS NVARCHAR(20),
	 @SortAscending AS INT,
     @PageSize AS INT,
     @PageIndex AS INT,
     @TotalCount AS INT OUTPUT
AS
BEGIN
	-- redeclare variables to workaround query plan caching performance issues
	DECLARE @SearchTermInternal NVARCHAR(100) = @SearchTerm
	DECLARE @SectorFiltersInternal NVARCHAR(MAX) = @SectorFilters
	DECLARE @LevelFiltersInternal NVARCHAR(MAX) = @LevelFilters
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
		Sector,
		StandardLevel,
		TotalCount = COUNT(1) OVER()
	INTO
		#Results
	FROM 
		StandardSummary
	WHERE
		(
			@SearchTermInternal = '' OR
			(
				(StandardName LIKE '%' + @SearchTermInternal + '%' ) OR
				(StandardReference LIKE '%' + @SearchTermInternal + '%') OR
				(Sector LIKE '%' + @SearchTermInternal + '%')
			)
		)
		AND
		(
			@SectorFiltersInternal = '' OR
			Sector IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @SectorFiltersInternal, '|' ))
		)
		AND
		(
			@LevelFiltersInternal = '' OR
			CASE StandardLevel WHEN 0 THEN 'To be confirmed' ELSE CONVERT(VARCHAR, StandardLevel) END IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @LevelFiltersInternal, '|' ))
		)
	GROUP BY 
		StandardCode, StandardReference, StandardName, Sector, StandardLevel

	-- the total number of results is returned as an out parameter
	SELECT @TotalCount = (SELECT MAX(TotalCount) FROM #Results)
	
	-- the resultset contains the requested page of standards from the results sorted by the column and direction
	SELECT 
		StandardCode, 
		StandardReference, 
		StandardName, 
		ActiveApprentices, 
		RegisteredEPAOs,
		TotalCount = COUNT(1) OVER()
	FROM 
		#Results
	ORDER BY
		CASE WHEN @SortAscendingInternal = 0 THEN ''
		ELSE
           CASE 
				WHEN @SortColumnInternal = 'StandardName' THEN StandardName
				-- all dynamic order by columns must be the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @SortColumnInternal = 'ActiveApprentices' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(ActiveApprentices AS VARCHAR(6)))), 6)
				WHEN @SortColumnInternal = 'RegisteredEPAOs' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(RegisteredEPAOs AS VARCHAR(6)))), 6)
				ELSE RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(StandardCode AS VARCHAR(MAX)))), 6)
			END
		END ASC,
		CASE WHEN @SortAscendingInternal = 1 THEN ''
		ELSE
           CASE 
				WHEN @SortColumnInternal = 'StandardName' THEN StandardName
				-- all dynamic order by columns must be the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @SortColumnInternal = 'ActiveApprentices' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(ActiveApprentices AS VARCHAR(6)))), 6)
				WHEN @SortColumnInternal = 'RegisteredEPAOs' THEN RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(RegisteredEPAOs AS VARCHAR(6)))), 6)
				ELSE RIGHT(REPLICATE('0', 6) + LTRIM(RTRIM(CAST(StandardCode AS VARCHAR(MAX)))), 6)
			END
		END DESC
	OFFSET @Skip ROWS
	FETCH NEXT @PageSizeInternal ROWS ONLY
END