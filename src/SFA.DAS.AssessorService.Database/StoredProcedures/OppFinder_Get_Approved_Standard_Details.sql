CREATE PROCEDURE [dbo].[OppFinder_Get_Approved_Standard_Details]
	 @StandardReference AS NVARCHAR(10)
AS
BEGIN

	SELECT
		ss.StandardCode,
		ss.StandardName,
		JSON_VALUE(sc.StandardData, '$.OverviewOfRole') OverviewOfRole,
		ss.StandardLevel,
		ss.StandardReference,
		ss.Region,
		ss.Ordering,
		ss.Learners ActiveApprentices,
		ss.Assessments CompletedAssessments,
		ss.EndPointAssessors,
		JSON_QUERY(CASE ISNULL(ss.EndPointAssessorList, '') WHEN '' THEN '{"EPAOS":null}' ELSE ss.EndPointAssessorList END, '$.EPAOS') EndPointAssessorsNames,
		ss.Sector,
		JSON_VALUE(sc.StandardData, '$.Duration') TypicalDuration,
		JSON_VALUE(sc.StandardData, '$.PublishedDate') ApprovedForDelivery,
		JSON_VALUE(sc.StandardData, '$.MaxFunding') MaxFunding,
		JSON_VALUE(sc.StandardData, '$.Trailblazer') Trailblazer,
		JSON_VALUE(sc.StandardData, '$.StandardPageUrl') StandardPageUrl,
		JSON_VALUE(sc.StandardData, '$.EqaProviderName') EqaProviderName,
		JSON_VALUE(sc.StandardData, '$.EqaProviderContactName') EqaProviderContactName,
		JSON_VALUE(sc.StandardData, '$.EqaProviderContactEmail') EqaProviderContactEmail,
		JSON_VALUE(sc.StandardData, '$.EqaProviderWebLink') EqaProviderWebLink

	INTO 
		#Results
	FROM
		StandardSummary ss
			LEFT JOIN StandardCollation sc 
			ON ss.StandardCode = sc.StandardId
	WHERE 
		StandardReference = @StandardReference
		AND sc.IsLive = 1

	-- there may be duplicates in either the StandardCode or the StandardReference in which case the Standard with the latest ApprovedForDelivery will be returned
	SELECT TOP 1
		StandardCode, StandardName, OverviewOfRole, StandardLevel, StandardReference, 
		SUM(ActiveApprentices) TotalActiveApprentices, 
		SUM(CompletedAssessments) TotalCompletedAssessments, 
		Sector, TypicalDuration, ApprovedForDelivery, MaxFunding, Trailblazer, StandardPageUrl, EqaProviderName, EqaProviderContactEmail, EqaProviderWebLink
	INTO
		#Details
	FROM 
		#Results
	GROUP BY
		StandardCode, StandardName, OverviewOfRole, StandardLevel, StandardReference, 
		Sector, TypicalDuration, ApprovedForDelivery, MaxFunding, Trailblazer, StandardPageUrl, EqaProviderName, EqaProviderContactEmail, EqaProviderWebLink
	ORDER BY
		CONVERT(DATE, ApprovedForDelivery) DESC

	-- the first set is details about the standard
	SELECT
		StandardCode, StandardName, OverviewOfRole, StandardLevel, StandardReference, 
		TotalActiveApprentices, TotalCompletedAssessments, 
		Sector, TypicalDuration, ApprovedForDelivery, MaxFunding, Trailblazer, StandardPageUrl, EqaProviderName, EqaProviderContactEmail, EqaProviderWebLink
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
