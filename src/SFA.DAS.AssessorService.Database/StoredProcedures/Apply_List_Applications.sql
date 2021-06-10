CREATE PROCEDURE [dbo].[Apply_List_Applications]
	@sequenceNos VARCHAR(MAX),
	@organisationId AS NVARCHAR(12),
	@includedApplicationSequenceStatus AS NVARCHAR(MAX),
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
	DECLARE @Skip INT = (@pageIndex - 1) * @pageSize
	
	SELECT 
		ApplicationId,
		SequenceNo,
		OrganisationName,
		StandardName,
		StandardCode,
		StandardReference,
		Versions,
		SubmittedDate,
		FeedbackAddedDate,
		ClosedDate,
		SubmissionCount,
		ApplicationStatus,
		ApplicationType,
		ReviewStatus,
		FinancialStatus,
		FinancialGrade,
		SequenceStatus,
		TotalCount
	INTO 
		#Results
	FROM
		[dbo].[Apply_Func_Get_Applications] (@sequenceNos, @organisationId, @includedApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedReviewStatus)
	
	-- the total number of results is returned as an out parameter
	SELECT @totalCount = (SELECT MAX(TotalCount) FROM #Results)

	SELECT 
		ApplicationId,
		SequenceNo,
		OrganisationName,
		StandardName,
		StandardCode,
		StandardReference,
		Versions,
		SubmittedDate,
		FeedbackAddedDate,
		ClosedDate,
		SubmissionCount,
		ApplicationStatus,
		ApplicationType,
		ReviewStatus,
		FinancialStatus,
		FinancialGrade,		
		SequenceStatus
	FROM
		#Results
	ORDER BY
		CASE WHEN @sortAscending = 0 THEN ''
		ELSE
           CASE 
				WHEN @sortColumn = 'OrganisationName' THEN OrganisationName
				WHEN @sortColumn = 'StandardReference' THEN StandardReference
				WHEN @sortColumn = 'StandardName' THEN StandardName
				WHEN @sortColumn = 'FinancialStatus' THEN FinancialStatus				
				WHEN @sortColumn = 'Status' THEN ApplicationStatus
				-- all dynamic order by columns must be the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @sortColumn = 'SubmittedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, SubmittedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @sortColumn = 'FeedbackAddedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, FeedbackAddedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @sortColumn = 'ClosedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, ClosedDate, 127)) AS VARCHAR(20)))), 20)
				ELSE CAST(ApplicationId AS VARCHAR(36))
			END
		END ASC,
		CASE WHEN @sortAscending = 1 THEN ''
		ELSE
           CASE 
				WHEN @sortColumn = 'OrganisationName' THEN OrganisationName
				WHEN @sortColumn = 'StandardReference' THEN StandardReference
				WHEN @sortColumn = 'StandardName' THEN StandardName
				WHEN @sortColumn = 'FinancialStatus' THEN FinancialStatus				
				WHEN @sortColumn = 'Status' THEN ApplicationStatus
				-- all dynamic order by columns must be the same type and using right aligned zero padded strings to sort as natural numbers
				WHEN @sortColumn = 'SubmittedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, SubmittedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @sortColumn = 'FeedbackAddedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, FeedbackAddedDate, 127)) AS VARCHAR(20)))), 20)
				WHEN @sortColumn = 'ClosedDate' THEN RIGHT(REPLICATE('0', 20) + LTRIM(RTRIM(CAST([dbo].[ToTicks](CONVERT(DATETIME2, ClosedDate, 127)) AS VARCHAR(20)))), 20)
				ELSE CAST(ApplicationId AS VARCHAR(36))
			END
		END DESC
	OFFSET @Skip ROWS
	FETCH NEXT @pageSize ROWS ONLY
END
