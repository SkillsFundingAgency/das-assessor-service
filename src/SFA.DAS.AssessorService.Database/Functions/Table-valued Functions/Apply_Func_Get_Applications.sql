CREATE FUNCTION [dbo].[Apply_Func_Get_Applications]
(	
	@sequenceNo INT,
	@organisationId AS NVARCHAR(12),
	@includedApplicationSequenceStatus AS NVARCHAR(MAX),
	@excludedApplicationStatus AS NVARCHAR(MAX),
	@excludedReviewStatus AS NVARCHAR(MAX),
	@includedReviewStatus AS NVARCHAR(MAX)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
        ap1.Id AS ApplicationId,
        seq.SequenceNo AS SequenceNo,
        org.EndPointAssessorName AS OrganisationName,
        CASE WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.StandardName')
		     ELSE NULL
        END As StandardName,
        CASE WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.StandardCode')
		     ELSE NULL
        END As StandardCode,
		CASE WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.StandardReference')
		     ELSE NULL
        END As StandardReference,
		CASE WHEN seq.SequenceNo = [dbo].[ApplyConst_ORGANISATION_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.LatestInitSubmissionDate')
		     WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.LatestStandardSubmissionDate')
			 WHEN seq.SequenceNo = [dbo].[ApplyConst_ORGANISATION_WITHDRAWAL_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.LatestOrganisationWithdrawalSubmissionDate')
		     WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_WITHDRAWAL_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.LatestStandardWithdrawalSubmissionDate')
		     ELSE NULL
		END As SubmittedDate,
        CASE WHEN seq.SequenceNo = [dbo].[ApplyConst_ORGANISATION_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.InitSubmissionFeedbackAddedDate')
		     WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.StandardSubmissionFeedbackAddedDate')
			 WHEN seq.SequenceNo = [dbo].[ApplyConst_ORGANISATION_WITHDRAWAL_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.OrganisationWithdrawalSubmissionFeedbackAddedDate')
		     WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_WITHDRAWAL_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.StandardWithdrawalSubmissionFeedbackAddedDate')
		     ELSE NULL
		END As FeedbackAddedDate,
		CASE WHEN seq.SequenceNo = [dbo].[ApplyConst_ORGANISATION_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.InitSubmissionClosedDate')
		     WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.StandardSubmissionClosedDate')
			 WHEN seq.SequenceNo = [dbo].[ApplyConst_ORGANISATION_WITHDRAWAL_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.OrganisationWithdrawalSubmissionClosedDate')
		     WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_WITHDRAWAL_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.StandardWithdrawalSubmissionClosedDate')
		     ELSE NULL
	    END As ClosedDate,
        CASE WHEN seq.SequenceNo = [dbo].[ApplyConst_ORGANISATION_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.InitSubmissionsCount')
		     WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.StandardSubmissionsCount')
			 WHEN seq.SequenceNo = [dbo].[ApplyConst_ORGANISATION_WITHDRAWAL_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.OrganisationWithdrawalSubmissionsCount')
		     WHEN seq.SequenceNo = [dbo].[ApplyConst_STANDARD_WITHDRAWAL_SEQUENCE_NO]() THEN JSON_VALUE(ap1.ApplyData, '$.Apply.StandardWithdrawalSubmissionsCount')
		     ELSE 0
		END As SubmissionCount,
        ap1.ApplicationStatus As ApplicationStatus,
        ap1.ReviewStatus As ReviewStatus,
        ap1.FinancialReviewStatus As FinancialStatus,
        JSON_VALUE(ap1.FinancialGrade,'$.SelectedGrade') AS FinancialGrade,
		JSON_VALUE(ap1.GovernanceRecommendation,'$.SelectedRecommendation') AS GovernanceRecommendation,
        seq.Status As SequenceStatus,
		TotalCount = COUNT(1) OVER()
	FROM 
		Apply ap1
		INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
        CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, Status VARCHAR(20), NotRequired BIT) seq
	WHERE 
		seq.SequenceNo = @sequenceNo AND seq.NotRequired = 0
		AND (org.EndPointAssessorOrganisationId = @organisationId OR @organisationId IS NULL)
		AND seq.Status IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @includedApplicationSequenceStatus, '|' )) 
        AND ap1.DeletedAt IS NULL
        AND ap1.ApplicationStatus NOT IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @excludedApplicationStatus, '|' )) 
        AND ap1.ReviewStatus NOT IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @excludedReviewStatus, '|' ))
		AND (ap1.ReviewStatus IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT ( @includedReviewStatus, '|' )) OR @includedReviewStatus IS NULL)
)

