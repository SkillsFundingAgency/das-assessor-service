CREATE PROCEDURE [dbo].[Apply_Get_ReviewStatusCounts]
	@includedNewApplicationSequenceStatus AS NVARCHAR(MAX),
    @includedInProgressApplicationSequenceStatus AS NVARCHAR(MAX),
    @includedHasFeedbackApplicationSequenceStatus AS NVARCHAR(MAX),
    @includedApprovedApplicationSequenceStatus AS NVARCHAR(MAX),
	@excludedApplicationStatus AS NVARCHAR(MAX),
	@excludedReviewStatus AS NVARCHAR(MAX),
	@includedNewReviewStatus  AS NVARCHAR(MAX),
	@includedInProgressReviewStatus AS NVARCHAR(MAX),
	@includedHasFeedbackReviewStatus AS NVARCHAR(MAX),
    @includedApprovedReviewStatus AS NVARCHAR(MAX)
AS
BEGIN
	SELECT ReviewStatus, Total
	FROM
	(
		SELECT @includedNewReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (1, null, @includedNewApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedNewReviewStatus)
		UNION ALL
		SELECT @includedInProgressReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (1, null, @includedInProgressApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedInProgressReviewStatus)
		UNION ALL
		SELECT @includedHasFeedbackReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (1, null, @includedHasFeedbackApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedHasFeedbackReviewStatus)
		UNION ALL
		SELECT @includedApprovedReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (1, null, @includedApprovedApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedApprovedReviewStatus)
	) [OrganisationApplications]
	
	SELECT ReviewStatus, Total
	FROM
	(
		SELECT @includedNewReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (2, null, @includedNewApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedNewReviewStatus)
		UNION ALL
		SELECT @includedInProgressReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (2, null, @includedInProgressApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedInProgressReviewStatus)
		UNION ALL
		SELECT @includedHasFeedbackReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (2, null, @includedHasFeedbackApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedHasFeedbackReviewStatus)
		UNION ALL
		SELECT @includedApprovedReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (2, null, @includedApprovedApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedApprovedReviewStatus)
	) [StandardApplications]

END
