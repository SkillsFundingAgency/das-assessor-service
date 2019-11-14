CREATE PROCEDURE [dbo].[Apply_List_Applications]
	@sequenceNo INT,
	@organisationId AS NVARCHAR(12),
	@sequenceStatus AS NVARCHAR(MAX),
	@excludedApplicationStatus AS NVARCHAR(MAX),
	@excludedReviewStatus AS NVARCHAR(MAX),
	@includedReviewStatus AS NVARCHAR(MAX),
	@sortColumn AS NVARCHAR(20),
	@sortAscending AS INT,
    @pageSize AS INT,
    @pageIndex AS INT,
    @totalCount AS INT OUTPUT
AS
BEGIN
	-- redeclare variables to workaround query plan caching performance issues
	DECLARE @SequenceNoInternal INT = @sequenceNo
	DECLARE @OrganisationIdInternal INT = @organisationId
	DECLARE @SequenceStatusInternal AS NVARCHAR(MAX) = @sequenceStatus
	DECLARE @ExcludedApplicationStatusInternal AS NVARCHAR(MAX) = @excludedApplicationStatus
	DECLARE @ExcludedReviewStatusInternal AS NVARCHAR(MAX) = @excludedReviewStatus
	DECLARE @IncludedReviewStatusInternal AS NVARCHAR(MAX) = @includedReviewStatus
	DECLARE @SortColumnInternal NVARCHAR(20) = @SortColumn
	DECLARE @SortAscendingInternal INT = @SortAscending
	DECLARE @PageSizeInternal INT = @PageSize
	DECLARE @PageIndexInternal INT = @PageIndex

	DECLARE @Skip INT = (@PageIndexInternal - 1) * @PageSizeInternal
	
	SELECT 
		ApplicationId,
		SequenceNo,
		OrganisationName,
		StandardName,
		StandardCode,
		StandardReference,
		SubmittedDate,
		FeedbackAddedDate,
		ClosedDate,
		SubmissionCount,
		ApplicationStatus,
		ReviewStatus,
		FinancialStatus,
		FinancialGrade,
		SequenceStatus,
		TotalCount
	INTO 
		#Results
	FROM
		[dbo].[Apply_Func_Get_Applications] (@SequenceNoInternal, @OrganisationIdInternal, @SequenceStatusInternal, @ExcludedApplicationStatusInternal, @ExcludedReviewStatusInternal, @IncludedReviewStatusInternal)
	
	-- the total number of results is returned as an out parameter
	SELECT @totalCount = (SELECT MAX(TotalCount) FROM #Results)

	SELECT 
		ApplicationId,
		SequenceNo,
		OrganisationName,
		StandardName,
		StandardCode,
		StandardReference,
		SubmittedDate,
		FeedbackAddedDate,
		ClosedDate,
		SubmissionCount,
		ApplicationStatus,
		ReviewStatus,
		FinancialStatus,
		FinancialGrade,
		SequenceStatus
	FROM
		#Results
	ORDER BY
		CASE WHEN @SortAscendingInternal = 0 THEN ''
		ELSE
           CASE 
				WHEN @SortColumnInternal = 'OrganisationName' THEN OrganisationName
				WHEN @SortColumnInternal = 'StandardReference' THEN StandardReference
				WHEN @SortColumnInternal = 'StandardName' THEN StandardName
				WHEN @SortColumnInternal = 'FinancialStatus' THEN FinancialStatus
				WHEN @SortColumnInternal = 'Status' THEN ApplicationStatus
				-- all dynamic order by columns must be the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @SortColumnInternal = 'SubmittedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, SubmittedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @SortColumnInternal = 'FeedbackAddedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, FeedbackAddedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @SortColumnInternal = 'ClosedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, ClosedDate, 127)) AS VARCHAR(20)))), 20)
				ELSE CAST(ApplicationId AS VARCHAR(36))
			END
		END ASC,
		CASE WHEN @SortAscendingInternal = 1 THEN ''
		ELSE
           CASE 
				WHEN @SortColumnInternal = 'OrganisationName' THEN OrganisationName
				WHEN @SortColumnInternal = 'StandardReference' THEN StandardReference
				WHEN @SortColumnInternal = 'StandardName' THEN StandardName
				WHEN @SortColumnInternal = 'FinancialStatus' THEN FinancialStatus
				WHEN @SortColumnInternal = 'Status' THEN ApplicationStatus
				-- all dynamic order by columns must be the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @SortColumnInternal = 'SubmittedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, SubmittedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @SortColumnInternal = 'FeedbackAddedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, FeedbackAddedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @SortColumnInternal = 'ClosedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, ClosedDate, 127)) AS VARCHAR(20)))), 20)
				ELSE CAST(ApplicationId AS VARCHAR(36))
			END
		END DESC
	OFFSET @Skip ROWS
	FETCH NEXT @PageSizeInternal ROWS ONLY
END
