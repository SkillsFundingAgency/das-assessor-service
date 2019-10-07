-- ApplicationSummary view
CREATE VIEW ApplicationSummary
AS
SELECT
    org.EndPointAssessorName AS OrganisationName,
    ap1.Id AS ApplicationId,
    CASE WHEN ap1.StandardCode IS NULL THEN 1 ELSE 2 END AS SequenceNo,
    CASE WHEN ap1.StandardCode  IS NULL THEN 'Midpoint' ELSE 'Standard' END AS ApplicationType,
    JSON_VALUE(ap1.Applydata,'$.Apply.StandardName') AS StandardName,
    JSON_VALUE(ap1.Applydata,'$.Apply.StandardCode') AS StandardCode,
    ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate'),JSON_VALUE(ap1.Applydata,'$.Apply.LatestInitSubmissionDate')) AS SubmittedDate,
    ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.StandardSubmissionsCount'),JSON_VALUE(ap1.Applydata,'$.Apply.InitSubmissionCount')) AS SubmissionCount,
    ap1.FinancialReviewStatus AS FinancialStatus,
    JSON_VALUE(ap1.FinancialGrade,'$.SelectedGrade') AS FinancialGrade,
    ap1.ApplicationStatus AS CurrentStatus,
    ap1.ReviewStatus AS ReviewStatus
FROM Apply ap1
INNER JOIN Organisations org ON ap1.OrganisationId = org.Id


