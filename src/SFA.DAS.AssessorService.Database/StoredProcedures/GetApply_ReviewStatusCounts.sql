CREATE PROCEDURE [dbo].[GetApply_ReviewStatusCounts]
	@openSequenceStatus AS NVARCHAR(MAX),
	@feedbackSequenceStatus AS NVARCHAR(MAX),
	@closedSequenceStatus AS NVARCHAR(MAX),
	@excludedApplicationStatus AS NVARCHAR(MAX),
	@excludedReviewStatus AS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @OpenOrganisationApplications AS ApplicationSummaryTbl
	DECLARE @FeedbackOrganisationApplications AS ApplicationSummaryTbl
	DECLARE @ClosedOrganisationApplications AS ApplicationSummaryTbl
	DECLARE @OpenStandardApplications AS ApplicationSummaryTbl
	DECLARE @FeedbackStandardApplications AS ApplicationSummaryTbl
	DECLARE @ClosedStandardApplications AS ApplicationSummaryTbl
	
	INSERT @OpenOrganisationApplications EXEC GetApply_Applications 1, @openSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus
	INSERT @FeedbackOrganisationApplications EXEC GetApply_Applications 1, @feedbackSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus
	INSERT @ClosedOrganisationApplications EXEC GetApply_Applications 1, @closedSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus

	INSERT @OpenStandardApplications EXEC GetApply_Applications 2, @openSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus
	INSERT @FeedbackStandardApplications EXEC GetApply_Applications 2, @feedbackSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus
	INSERT @ClosedStandardApplications EXEC GetApply_Applications 2, @closedSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus
	
	SELECT ReviewStatus, COUNT(*) Total
	FROM
	(
		SELECT * FROM @OpenOrganisationApplications
		UNION
		SELECT * FROM @FeedbackOrganisationApplications
		UNION
		SELECT * FROM @ClosedOrganisationApplications
	) [OrganisationApplications]
	GROUP BY ReviewStatus

	SELECT ReviewStatus, COUNT(*) Total
	FROM
	(
		SELECT * FROM @OpenStandardApplications
		UNION
		SELECT * FROM @FeedbackStandardApplications
		UNION
		SELECT * FROM @ClosedStandardApplications
	) [StandardApplications]
	GROUP BY ReviewStatus
END