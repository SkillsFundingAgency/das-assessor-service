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
	DECLARE @Skip INT = (@PageIndex - 1) * @PageSize
	
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
		[dbo].[Apply_Func_Get_Applications] (@SequenceNo, @OrganisationId, @SequenceStatus, @ExcludedApplicationStatus, @ExcludedReviewStatus, @IncludedReviewStatus)
	
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
		CASE WHEN @SortAscending = 0 THEN ''
		ELSE
           CASE 
				WHEN @SortColumn = 'OrganisationName' THEN OrganisationName
				WHEN @SortColumn = 'StandardReference' THEN StandardReference
				WHEN @SortColumn = 'StandardName' THEN StandardName
				WHEN @SortColumn = 'FinancialStatus' THEN FinancialStatus
				WHEN @SortColumn = 'Status' THEN ApplicationStatus
				-- all dynamic order by columns must be the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @SortColumn = 'SubmittedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, SubmittedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @SortColumn = 'FeedbackAddedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, FeedbackAddedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @SortColumn = 'ClosedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, ClosedDate, 127)) AS VARCHAR(20)))), 20)
				ELSE CAST(ApplicationId AS VARCHAR(36))
			END
		END ASC,
		CASE WHEN @SortAscending = 1 THEN ''
		ELSE
           CASE 
				WHEN @SortColumn = 'OrganisationName' THEN OrganisationName
				WHEN @SortColumn = 'StandardReference' THEN StandardReference
				WHEN @SortColumn = 'StandardName' THEN StandardName
				WHEN @SortColumn = 'FinancialStatus' THEN FinancialStatus
				WHEN @SortColumn = 'Status' THEN ApplicationStatus
				-- all dynamic order by columns must be the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @SortColumn = 'SubmittedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, SubmittedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @SortColumn = 'FeedbackAddedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, FeedbackAddedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @SortColumn = 'ClosedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, ClosedDate, 127)) AS VARCHAR(20)))), 20)
				ELSE CAST(ApplicationId AS VARCHAR(36))
			END
		END DESC
	OFFSET @Skip ROWS
	FETCH NEXT @PageSize ROWS ONLY
END
