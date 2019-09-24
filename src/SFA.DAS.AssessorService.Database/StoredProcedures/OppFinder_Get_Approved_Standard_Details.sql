CREATE PROCEDURE [dbo].[OppFinder_Get_Approved_Standard_Details]
	 @StandardCode AS INT
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
	ORDER BY 
		ss.StandardReference

	SELECT
		StandardName, OverviewOfRole, StandardLevel, StandardReference, 
		SUM(ActiveApprentices) TotalActiveApprentices, 
		SUM(CompletedAssessments) TotalCompletedAssessments, 
		Sector, TypicalDuration, ApprovedForDelivery, MaxFunding, Trailblazer, StandardPageUrl, EqaProviderName, EqaProviderContactEmail, EqaProviderWebLink
	FROM 
		#Results
	WHERE 
		StandardCode = @StandardCode
	GROUP BY
		StandardName, OverviewOfRole, StandardLevel, StandardReference, 
		Sector, TypicalDuration, ApprovedForDelivery, MaxFunding, Trailblazer, StandardPageUrl, EqaProviderName, EqaProviderContactEmail, EqaProviderWebLink

	SELECT 
		Region, EndPointAssessorsNames, EndPointAssessors, ActiveApprentices, CompletedAssessments
	FROM 
		#Results
	WHERE 
		StandardCode = @StandardCode
	ORDER BY 
		Ordering
END
