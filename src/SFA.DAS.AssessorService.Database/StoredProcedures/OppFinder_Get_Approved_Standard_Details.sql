CREATE PROCEDURE [dbo].[OppFinder_Get_Approved_Standard_Details]
	 @StandardReference AS NVARCHAR(10)
AS
BEGIN

	SELECT
		ss.StandardCode,
		ss.StandardName,
		s.OverviewOfRole,
		ss.StandardLevel,
		ss.StandardReference,
		ss.Region,
		ss.Ordering,
		ss.Learners ActiveApprentices,
		ss.Assessments CompletedAssessments,
		ss.EndPointAssessors,
		JSON_QUERY(CASE ISNULL(ss.EndPointAssessorList, '') WHEN '' THEN '{"EPAOS":null}' ELSE ss.EndPointAssessorList END, '$.EPAOS') EndPointAssessorsNames,
		ss.Sector,
		s.TypicalDuration,
		s.VersionApprovedForDelivery AS ApprovedForDelivery,
		s.MaxFunding,
		s.TrailBlazerContact AS Trailblazer,
		s.StandardPageUrl,
		s.EqaProviderName,
		s.EqaProviderContactName,
		s.EqaProviderContactEmail
	INTO 
		#Results
	FROM
		StandardSummary ss
			LEFT JOIN 
				(SELECT [LarsCode], [IFateReferenceNumber], [OverviewOfRole], [TypicalDuration], [VersionApprovedForDelivery],
				[MaxFunding], [TrailBlazerContact], [StandardPageUrl], [EqaProviderName], [EqaProviderContactName], [EqaProviderContactEmail],
				ROW_NUMBER() OVER(PARTITION BY [IFateReferenceNumber] ORDER BY [Version] DESC) AS RowNum
		  FROM [Standards]
		) s ON ss.StandardCode = s.LarsCode AND s.RowNum = 1
	WHERE 
		StandardReference = @StandardReference

	-- there may be duplicates in either the StandardCode or the StandardReference in which case the Standard with the latest ApprovedForDelivery will be returned
	SELECT TOP 1
		StandardCode, StandardName, OverviewOfRole, StandardLevel, StandardReference, 
		SUM(ActiveApprentices) TotalActiveApprentices, 
		SUM(CompletedAssessments) TotalCompletedAssessments, 
		Sector, TypicalDuration, ApprovedForDelivery, MaxFunding, Trailblazer, StandardPageUrl, EqaProviderName, EqaProviderContactEmail
	INTO
		#Details
	FROM 
		#Results
	GROUP BY
		StandardCode, StandardName, OverviewOfRole, StandardLevel, StandardReference, 
		Sector, TypicalDuration, ApprovedForDelivery, MaxFunding, Trailblazer, StandardPageUrl, EqaProviderName, EqaProviderContactEmail
	ORDER BY
		CONVERT(DATE, ApprovedForDelivery) DESC

	-- the first set is details about the standard
	SELECT
		StandardCode, StandardName, OverviewOfRole, StandardLevel, StandardReference, 
		TotalActiveApprentices, TotalCompletedAssessments, 
		Sector, TypicalDuration, ApprovedForDelivery, MaxFunding, Trailblazer, StandardPageUrl, EqaProviderName, EqaProviderContactEmail
	FROM
		#Details

	-- the second set is details about the regions for which EPAO's currently assess the standard
	SELECT 
		Region, EndPointAssessorsNames, EndPointAssessors, ActiveApprentices, CompletedAssessments
	FROM 
		#Results [Results] INNER JOIN #Details [Details]
		ON [Results].StandardCode = [Details].StandardCode
	ORDER BY 
		Ordering

	-- the third set is details about the standard versions
	SELECT
		Version, ActiveApprentices, CompletedAssessments, EndPointAssessors
	FROM
		StandardVersionSummary s
	WHERE 
		StandardReference = @StandardReference
	ORDER BY 
		Version DESC
END
