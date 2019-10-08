CREATE PROCEDURE [dbo].OppFinder_List_NonApproved_Standards
	 @SearchTerm AS NVARCHAR(100),
	 @SectorFilters AS NVARCHAR(MAX),
	 @LevelFilters AS NVARCHAR(MAX),
	 @SortColumn AS NVARCHAR(20),
	 @SortAscending AS INT,
     @PageSize AS INT,
     @PageIndex AS INT,
	 @NonApprovedType NVARCHAR(15),
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

	-- there are some specifically excluded Standards
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
		TotalCount = COUNT(1) OVER()
	INTO
		#Results
	FROM 
		StandardNonApprovedCollation
	LEFT JOIN 
		@Exclusions Exclusions
		ON Exclusions.StandardReference = StandardNonApprovedCollation.ReferenceNumber
	WHERE
		Exclusions.StandardReference IS NULL AND
		(
			(@NonApprovedType = 'InDevelopment' AND
				(
					[dbo].OppFinder_Is_InDevelopment_StandardStatus(StandardData) = 1 OR
					-- when an Approved standard is in the [StandardNonApprovedCollation] (because it has no StandardId) it counts as an InDevelopment standard
					[dbo].OppFinder_Is_Approved_StandardStatus(StandardData) = 1
				)
			) OR
			(@NonApprovedType = 'Proposed' AND [dbo].OppFinder_Is_Proposed_StandardStatus(StandardData) = 1)
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
			JSON_VALUE(StandardData, '$.Category') IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @SectorFiltersInternal, '|' ))
		)
		AND
		(
			@LevelFiltersInternal = '' OR
			CASE JSON_VALUE(StandardData, '$.Level') WHEN 0 THEN 'To be confirmed' ELSE CONVERT(VARCHAR, JSON_VALUE(StandardData, '$.Level')) END IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @LevelFiltersInternal, '|' ))
		)
		AND IsLive = 1
	GROUP BY 
		ReferenceNumber, Title, StandardData

	SELECT @TotalCount = (SELECT MAX(TotalCount) FROM #Results)

	SELECT 
		Results.StandardReference, 
		Results.StandardName
	FROM 
		#Results Results
	ORDER BY -- ensure that results are ordered on final select from temporary table
		CASE WHEN @SortAscendingInternal = 0 THEN ''
		ELSE
			Results.StandardName
		END ASC,
		CASE WHEN @SortAscendingInternal = 1 THEN ''
		ELSE
			Results.StandardName
		END DESC
	OFFSET @Skip ROWS
	FETCH NEXT @PageSizeInternal ROWS ONLY
END