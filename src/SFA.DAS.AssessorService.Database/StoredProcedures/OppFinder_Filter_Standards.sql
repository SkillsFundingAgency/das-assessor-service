CREATE PROCEDURE [dbo].[OppFinder_Filter_Standards]
	 @SearchTerm AS NVARCHAR(100),
	 @SectorFilters AS NVARCHAR(MAX),
	 @LevelFilters AS NVARCHAR(MAX)
AS
BEGIN
	-- variables redeclared to workaround query plan parameter sniffing performance issues
	-- this is now regarded as bad practice and should be removed
	DECLARE @SearchTermInternal NVARCHAR(100) = @SearchTerm
	DECLARE @SectorFiltersInternal NVARCHAR(MAX) = ISNULL(@SectorFilters, '')
	DECLARE @LevelFiltersInternal NVARCHAR(MAX) = ISNULL(@LevelFilters, '')

	-- there are some specifically excluded Standards
	DECLARE @Exclusions TABLE
	(
		StandardName nvarchar(500),
		StandardReference nvarchar(10)
	) 
	
	INSERT INTO @Exclusions(StandardName, StandardReference)
	EXEC OppFinder_Exclusions 

	-- pre-filtered standards 
	DECLARE @StandardsBase TABLE
	(
		 StandardReference nvarchar(10) NOT NULL,
		 StandardName nvarchar(500) NOT NULL,
		 StandardLevel varchar(20) NULL,
		 Sector nvarchar(500) NOT NULL
	);

	INSERT INTO @StandardsBase (StandardReference, StandardName, StandardLevel, Sector)
	SELECT stv.StandardReference, stv.StandardName
		  ,CASE StandardLevel WHEN 0 THEN 'To be confirmed' ELSE CONVERT(VARCHAR, StandardLevel) END StandardLevel, Sector
	FROM (
		SELECT st1.IFateReferenceNumber StandardReference
			  ,Title StandardName
			  ,Level StandardLevel
			  ,Route Sector
			  ,ROW_NUMBER() OVER(PARTITION BY st1.IFateReferenceNumber ORDER BY VersionMajor DESC, VersionMinor DESC) AS RowNumber
		FROM Standards st1
		WHERE 1=1
		  AND Status IN ('In development','Proposal in development','Approved for delivery' )
		  AND ISNULL(IntegratedDegree, '') <> 'integrated degree'
	) stv 
	LEFT JOIN @Exclusions ex1 ON ex1.StandardReference = stv.StandardReference
	WHERE RowNumber = 1
	  AND ex1.StandardName IS NULL

	  
	-- get sector and standard level for approved, in development and proposed standards
	SELECT StandardReference, StandardName, Sector, StandardLevel
	INTO
		#SearchResults
	FROM @StandardsBase
	WHERE 1 = 1
	  AND 
		(	@SearchTerm = '' OR
			(
				(StandardName LIKE '%' + @SearchTerm + '%' ) OR
				(StandardReference LIKE '%' + @SearchTerm + '%') OR
				(Sector LIKE '%' + @SearchTerm + '%')
			)
		)

	-- apply a sector level filter to the search results
	SELECT 
		StandardReference, 
		StandardName, 
		Sector, 
		StandardLevel
	INTO 
		#SearchResultsSectorFilter
	FROM 
		#SearchResults
	WHERE
		@SectorFiltersInternal = '' OR
		Sector IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @SectorFiltersInternal, '|' ))
	
	-- apply a standard level filter to the search results	
	SELECT 
		StandardReference, 
		StandardName, 
		Sector, 
		StandardLevel
	INTO 
		#SearchResultsLevelFilter
	FROM 
		#SearchResults
	WHERE
		@LevelFiltersInternal = '' OR
		StandardLevel IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @LevelFiltersInternal, '|' ))
			
	-- first resultset contains the number of Standards per Sector from both approved and non approved Standards which match each Level filter
	SELECT Sector, SUM(MatchingSectorFilter) MatchingSectorFilter
	FROM
		(
			(
				SELECT Sector, 	COUNT(*) MatchingSectorFilter 
				FROM #SearchResultsLevelFilter GROUP BY Sector	
			)
			UNION ALL
			(
				SELECT Sector, 0 MatchingSectoryFilter 
				FROM @StandardsBase GROUP BY Sector
			)
		) 
		[AllResults]
	GROUP BY
		[AllResults].Sector
	
	-- second resultset contains the number of Standards per Level from both approved and non approved Standards which match each Sector filter
	SELECT StandardLevel, SUM(MatchingLevelFilter) MatchingLevelFilter
	FROM
		(
			(
				SELECT StandardLevel, COUNT(*) MatchingLevelFilter 
				FROM #SearchResultsSectorFilter GROUP BY StandardLevel
			)
			UNION ALL
			(
				SELECT StandardLevel, 0 MatchingLevelFilter 
				FROM @StandardsBase GROUP BY StandardLevel
			)
		)
		[AllResults]
	GROUP BY
		[AllResults].StandardLevel
END