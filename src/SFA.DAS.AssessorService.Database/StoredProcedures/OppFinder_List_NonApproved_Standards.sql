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
	-- variables redeclared to workaround query plan parameter sniffing performance issues
	-- this is now regarded as bad practice and should be removed
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

	-- pre-filtered standards 
	DECLARE @StandardsBase TABLE
	(
		 StandardReference nvarchar(10) NOT NULL,
		 StandardName nvarchar(500) NOT NULL,
		 StandardLevel varchar(20) NULL,
		 Sector nvarchar(500) NOT NULL,
         Status varchar(100) NOT NULL
	);

	INSERT INTO @StandardsBase (StandardReference, StandardName, StandardLevel, Sector, Status)
	SELECT stv.StandardReference, stv.StandardName
		  ,CASE StandardLevel WHEN 0 THEN 'To be confirmed' ELSE CONVERT(VARCHAR, StandardLevel) END StandardLevel, Sector, Status
	FROM (
		SELECT st1.IFateReferenceNumber StandardReference
			  ,Title StandardName
              ,Level StandardLevel
			  ,Route Sector
              ,Status
			  ,ROW_NUMBER() OVER(PARTITION BY st1.IFateReferenceNumber ORDER BY VersionMajor DESC, VersionMinor DESC) AS RowNumber
		FROM Standards st1
		WHERE 1=1
		  AND Status IN ('In development','Proposal in development' )
          AND VersionApprovedForDelivery IS NULL
		  AND ISNULL(IntegratedDegree, '') <> 'integrated degree'
	) stv 
	LEFT JOIN @Exclusions ex1 ON ex1.StandardReference = stv.StandardReference
	WHERE RowNumber = 1
	  AND ex1.StandardName IS NULL
      
	SELECT 
		StandardReference, 
		StandardName, 
		TotalCount = COUNT(1) OVER()
	INTO
		#Results
	FROM 
		@StandardsBase
	WHERE
		1 = 1 AND
		(
			(@NonApprovedType = 'InDevelopment' AND Status = 'In development' )
			 OR
			(@NonApprovedType = 'Proposed' AND Status = 'Proposal in development' )
		)
		AND
		(	@SearchTerm = '' OR
			(
				(StandardName LIKE '%' + @SearchTerm + '%' ) OR
				(StandardReference LIKE '%' + @SearchTerm + '%') OR
				(Sector LIKE '%' + @SearchTerm + '%')
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
			StandardLevel IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @LevelFiltersInternal, '|' ))
		)
	GROUP BY 
		StandardReference, StandardName

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