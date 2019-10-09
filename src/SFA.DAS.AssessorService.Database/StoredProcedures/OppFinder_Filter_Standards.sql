CREATE PROCEDURE [dbo].[OppFinder_Filter_Standards]
	 @SearchTerm AS NVARCHAR(100),
	 @SectorFilters AS NVARCHAR(MAX),
	 @LevelFilters AS NVARCHAR(MAX)
AS
BEGIN
	-- redeclare variables to workaround query plan caching performance issues
	DECLARE @SearchTermInternal NVARCHAR(100) = @SearchTerm
	DECLARE @SectorFiltersInternal NVARCHAR(MAX) = ISNULL(@SectorFilters, '')
	DECLARE @LevelFiltersInternal NVARCHAR(MAX) = ISNULL(@LevelFilters, '')

	-- get sector and standard level from approved standards
	SELECT 
		StandardReference, 
		StandardName, 
		Sector,
		CASE StandardLevel WHEN 0 THEN 'To be confirmed' ELSE CONVERT(VARCHAR, StandardLevel) END StandardLevel
	INTO #ApprovedSearchResults FROM StandardSummary
	WHERE
		@SearchTermInternal = '' OR
		(
			(StandardName LIKE '%' + @SearchTermInternal + '%' ) OR
			(StandardReference LIKE '%' + @SearchTermInternal + '%') OR
			(Sector LIKE '%' + @SearchTermInternal + '%')
		)
	GROUP BY 
		StandardReference, StandardName, Sector, StandardLevel
	
	-- there are some specifically excluded non approved Standards
	DECLARE @Exclusions TABLE
	(
		[StandardName] nvarchar(500),
		[StandardReference] nvarchar(10)
	) 
	
	INSERT INTO @Exclusions(StandardName, StandardReference)
	EXEC OppFinder_Exclusions 

	-- get the sector and standard level from non approved standards
	SELECT 
		ReferenceNumber StandardReference, 
		Title StandardName,
		JSON_VALUE(StandardData, '$.Category') Sector,
		CASE JSON_VALUE(StandardData, '$.Level') WHEN 0 THEN 'To be confirmed' ELSE CONVERT(VARCHAR, JSON_VALUE(StandardData, '$.Level')) END StandardLevel
	INTO
		#NonApprovedSearchResults
	FROM 
		StandardNonApprovedCollation
	LEFT JOIN 
		@Exclusions Exclusions
		ON Exclusions.StandardReference = StandardNonApprovedCollation.ReferenceNumber
	WHERE
		Exclusions.StandardReference IS NULL AND
		(
			[dbo].OppFinder_Is_InDevelopment_StandardStatus(StandardData) = 1 OR
			-- when an Approved standard is in the [StandardNonApprovedCollation] (because it has no StandardId) it is returned in results
			[dbo].OppFinder_Is_Approved_StandardStatus(StandardData) = 1 OR
			[dbo].OppFinder_Is_Proposed_StandardStatus(StandardData) = 1
		)
		AND 
		(	@SearchTerm = '' OR
			(
				(Title LIKE '%' + @SearchTerm + '%' ) OR
				(ReferenceNumber LIKE '%' + @SearchTerm + '%') OR
				(JSON_VALUE(StandardData, '$.Category') LIKE '%' + @SearchTerm + '%')
			)
		)
		AND IsLive = 1
	GROUP BY 
		ReferenceNumber, Title, StandardData

	-- combine approved and non approved into a single search result
	SELECT 
		StandardReference, 
		StandardName, 
		Sector, 
		StandardLevel
	INTO
		#SearchResults
	FROM
	(
		(
			SELECT StandardReference, StandardName, Sector, StandardLevel
			FROM #ApprovedSearchResults
		) 
		UNION
		(
			SELECT StandardReference, StandardName, Sector, StandardLevel
			FROM #NonApprovedSearchResults
		) 
	) [SearchResults]

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
				FROM StandardSummary GROUP BY Sector
			)
			UNION ALL
			(
				SELECT JSON_VALUE(StandardData, '$.Category') Sector, 0 MatchingSectorFilter
				FROM StandardNonApprovedCollation GROUP BY JSON_VALUE(StandardData, '$.Category')
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
				SELECT CASE StandardLevel WHEN 0 THEN 'To be confirmed' ELSE CONVERT(VARCHAR, StandardLevel) END StandardLevel, 0 MatchingLevelFilter
				FROM StandardSummary GROUP BY CASE StandardLevel WHEN 0 THEN 'To be confirmed' ELSE CONVERT(VARCHAR, StandardLevel) END
			)
			UNION ALL
			(
				SELECT CASE JSON_VALUE(StandardData, '$.Level') WHEN 0 THEN 'To be confirmed' ELSE JSON_VALUE(StandardData, '$.Level') END StandardLevel, 0 MatchingLevelFilter
				FROM StandardNonApprovedCollation GROUP BY CASE JSON_VALUE(StandardData, '$.Level') WHEN 0 THEN 'To be confirmed' ELSE JSON_VALUE(StandardData, '$.Level') END
			)
		)
		[AllResults]
	GROUP BY
		[AllResults].StandardLevel
END