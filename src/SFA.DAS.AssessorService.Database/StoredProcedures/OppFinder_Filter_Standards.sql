CREATE PROCEDURE [dbo].[OppFinder_Filter_Standards]
	 @SearchTerm AS NVARCHAR(100),
	 @SectorFilters AS NVARCHAR(MAX),
	 @LevelFilters AS NVARCHAR(MAX)
AS
BEGIN
	-- redeclare variables to workaround query plan caching performance issues
	DECLARE @SearchTermInternal NVARCHAR(100) = @SearchTerm
	DECLARE @SectorFiltersInternal NVARCHAR(MAX) = @SectorFilters
	DECLARE @LevelFiltersInternal NVARCHAR(MAX) = @LevelFilters

	SELECT 
		StandardReference, 
		StandardName, 
		Sector,
		StandardLevel
	INTO
		#ApprovedResults
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
			Sector IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @SectorFiltersInternal, ',' ))
		)
		AND
		(
			@LevelFiltersInternal = '' OR
			CASE StandardLevel WHEN 0 THEN 'TBC' ELSE CONVERT(VARCHAR, StandardLevel) END IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @LevelFiltersInternal, ',' ))
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

	SELECT 
		ReferenceNumber StandardReference, 
		Title StandardName,
		JSON_VALUE(StandardData, '$.Category') Sector,
		JSON_VALUE(StandardData, '$.Level') StandardLevel
	INTO
		#NonApprovedResults
	FROM 
		StandardNonApprovedCollation
	LEFT JOIN 
		@Exclusions Exclusions
		ON Exclusions.StandardReference = StandardNonApprovedCollation.ReferenceNumber
	WHERE
		Exclusions.StandardReference IS NULL AND
		(
			[dbo].OppFinder_Is_InDevelopment_StandardStatus(StandardData) = 1 OR
			[dbo].OppFinder_Is_Approved_StandardStatus(StandardData) = 1 OR
			-- when an Approved standard is in the [StandardNonApprovedCollation] (because it has no StandardId) it is returned in results
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
		AND
		(
			@SectorFiltersInternal = '' OR
			JSON_VALUE(StandardData, '$.Category') IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @SectorFiltersInternal, ',' ))
		)
		AND
		(
			@LevelFiltersInternal = '' OR
			CASE JSON_VALUE(StandardData, '$.Level') WHEN 0 THEN 'TBC' ELSE CONVERT(VARCHAR, JSON_VALUE(StandardData, '$.Level')) END IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @LevelFiltersInternal, ',' ))
		)
		AND IsLive = 1
	GROUP BY 
		ReferenceNumber, Title, StandardData
	
	-- first resultset contains the number of Standards from both approved and non approved Standards which match each sector filter
	SELECT Sector, SUM(MatchingSectorFilter)
	FROM
		(
			(
				SELECT Sector, 	COUNT(*) MatchingSectorFilter 
				FROM #ApprovedResults GROUP BY Sector	
			)
			UNION ALL
			(
				SELECT Sector, COUNT(*) MatchingSectorFilter 
				FROM #NonApprovedResults GROUP BY Sector
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
	
	-- second resultset contains the number of Standards from both approved and non approved Standards which match each level filter
	SELECT StandardLevel, SUM(MatchingLevelFilter)
	FROM
		(
			(
				SELECT CASE StandardLevel WHEN 0 THEN 'TBC' ELSE CONVERT(VARCHAR, StandardLevel) END StandardLevel, COUNT(*) MatchingLevelFilter 
				FROM #ApprovedResults GROUP BY CASE StandardLevel WHEN 0 THEN 'TBC' ELSE CONVERT(VARCHAR, StandardLevel) END
			)
			UNION ALL
			(
				SELECT CASE StandardLevel WHEN 0 THEN 'TBC' ELSE CONVERT(VARCHAR, StandardLevel) END StandardLevel, COUNT(*) MatchingLevelFilter 
				FROM #NonApprovedResults GROUP BY CASE StandardLevel WHEN 0 THEN 'TBC' ELSE CONVERT(VARCHAR, StandardLevel) END
			)
			UNION ALL
			(
				SELECT CASE StandardLevel WHEN 0 THEN 'TBC' ELSE CONVERT(VARCHAR, StandardLevel) END StandardLevel, 0 MatchingLevelFilter
				FROM StandardSummary GROUP BY CASE StandardLevel WHEN 0 THEN 'TBC' ELSE CONVERT(VARCHAR, StandardLevel) END
			)
			UNION ALL
			(
				SELECT CASE JSON_VALUE(StandardData, '$.Level') WHEN 0 THEN 'TBC' ELSE JSON_VALUE(StandardData, '$.Level') END StandardLevel, 0 MatchingLevelFilter
				FROM StandardNonApprovedCollation GROUP BY CASE JSON_VALUE(StandardData, '$.Level') WHEN 0 THEN 'TBC' ELSE JSON_VALUE(StandardData, '$.Level') END
			)
		)
		[AllResults]
	GROUP BY
		[AllResults].StandardLevel
END