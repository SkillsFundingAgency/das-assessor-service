-- ApplicationSummary view
CREATE VIEW ApplicationSummary
AS
SELECT
	ap1.Id AS ApplicationId,
	CASE WHEN ap1.StandardCode IS NULL THEN 1 ELSE 2 END AS SequenceNo,
    org.EndPointAssessorName AS OrganisationName,
    CASE WHEN ap1.StandardCode  IS NULL THEN 'Midpoint' ELSE 'Standard' END AS ApplicationType,
    JSON_VALUE(ap1.Applydata,'$.Apply.StandardName') AS StandardName,
    JSON_VALUE(ap1.Applydata,'$.Apply.StandardCode') AS StandardCode,
    ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate'),JSON_VALUE(ap1.Applydata,'$.Apply.LatestInitSubmissionDate')) AS SubmittedDate,
	ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.StandardSubmissionFeedbackAddedDate'),JSON_VALUE(ap1.Applydata,'$.Apply.InitSubmissionFeedbackAddedDate')) AS FeedbackAddedDate,
	ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.StandardSubmissionClosedDate'),JSON_VALUE(ap1.Applydata,'$.Apply.InitSubmissionClosedDate')) AS ClosedDate,
    CASE WHEN JSON_VALUE(ap1.Applydata,'$.Apply.StandardSubmissionsCount') = 0 THEN JSON_VALUE(ap1.Applydata,'$.Apply.InitSubmissionsCount') ELSE JSON_VALUE(ap1.Applydata,'$.Apply.StandardSubmissionsCount') END AS SubmissionCount,
    ap1.ApplicationStatus AS ApplicationStatus,
	ap1.ReviewStatus AS ReviewStatus,
	ap1.FinancialReviewStatus AS FinancialStatus,
    JSON_VALUE(ap1.FinancialGrade,'$.SelectedGrade') AS FinancialGrade    
FROM Apply ap1
INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
WHERE ap1.DeletedAt IS NULL



