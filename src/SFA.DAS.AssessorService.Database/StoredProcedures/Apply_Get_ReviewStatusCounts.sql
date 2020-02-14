
CREATE PROCEDURE [dbo].[Apply_Get_ReviewStatusCounts]
	@includedApplicationSequenceStatus AS NVARCHAR(MAX),
	@excludedApplicationStatus AS NVARCHAR(MAX),
	@excludedReviewStatus AS NVARCHAR(MAX)
AS
BEGIN
	SELECT ReviewStatus, COUNT(*) Total
	FROM
	(
		SELECT * FROM [dbo].[Apply_Func_Get_Applications] (1, null, @includedApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, null)
	) [OrganisationApplications]
	GROUP BY ReviewStatus

	SELECT ReviewStatus, COUNT(*) Total
	FROM
	(
		SELECT * FROM [dbo].[Apply_Func_Get_Applications] (2, null, @includedApplicationSequenceStatus, @excludedApplicationStatus, @excludedReviewStatus, null)
	) [StandardApplications]
	GROUP BY ReviewStatus
END
