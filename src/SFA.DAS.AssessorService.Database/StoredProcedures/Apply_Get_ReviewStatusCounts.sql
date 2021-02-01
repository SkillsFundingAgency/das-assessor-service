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
	DECLARE @OrganisationSequenceNos VARCHAR(MAX) = CONVERT(VARCHAR, dbo.[ApplyConst_ORGANISATION_SEQUENCE_NO]())
	DECLARE @StandardSequenceNos VARCHAR(MAX) = CONVERT(VARCHAR, dbo.[ApplyConst_STANDARD_SEQUENCE_NO]())
	DECLARE @WithdrawalSequenceNos VARCHAR(MAX) = CONVERT(VARCHAR, dbo.[ApplyConst_ORGANISATION_WITHDRAWAL_SEQUENCE_NO]()) + '|' + CONVERT(VARCHAR, dbo.[ApplyConst_STANDARD_WITHDRAWAL_SEQUENCE_NO]())

	SELECT ReviewStatus, Total
	FROM
	(
		SELECT @includedNewReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@OrganisationSequenceNos, null, @includedNewApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedNewReviewStatus)
		UNION ALL
		SELECT @includedInProgressReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@OrganisationSequenceNos, null, @includedInProgressApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedInProgressReviewStatus)
		UNION ALL
		SELECT @includedHasFeedbackReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@OrganisationSequenceNos, null, @includedHasFeedbackApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedHasFeedbackReviewStatus)
		UNION ALL
		SELECT @includedApprovedReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@OrganisationSequenceNos, null, @includedApprovedApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedApprovedReviewStatus)
	) [OrganisationApplications]
	
	SELECT ReviewStatus, Total
	FROM
	(
		SELECT @includedNewReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@StandardSequenceNos, null, @includedNewApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedNewReviewStatus)
		UNION ALL
		SELECT @includedInProgressReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@StandardSequenceNos, null, @includedInProgressApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedInProgressReviewStatus)
		UNION ALL
		SELECT @includedHasFeedbackReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@StandardSequenceNos, null, @includedHasFeedbackApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedHasFeedbackReviewStatus)
		UNION ALL
		SELECT @includedApprovedReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@StandardSequenceNos, null, @includedApprovedApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedApprovedReviewStatus)
	) [StandardApplications]

	SELECT ReviewStatus, Total
	FROM
	(
		SELECT @includedNewReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@WithdrawalSequenceNos, null, @includedNewApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedNewReviewStatus)
		UNION ALL
		SELECT @includedInProgressReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@WithdrawalSequenceNos, null, @includedInProgressApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedInProgressReviewStatus)
		UNION ALL
		SELECT @includedHasFeedbackReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@WithdrawalSequenceNos, null, @includedHasFeedbackApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedHasFeedbackReviewStatus)
		UNION ALL
		SELECT @includedApprovedReviewStatus ReviewStatus, COUNT(ReviewStatus) Total FROM [dbo].[Apply_Func_Get_Applications] (@WithdrawalSequenceNos, null, @includedApprovedApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, @includedApprovedReviewStatus)
	) [WithdrawalApplications]

END
