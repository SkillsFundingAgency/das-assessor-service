CREATE PROCEDURE [GetEPAO_DashboardCounts]
      @epaOrgId NVARCHAR(12),
	  @pipelineCutOff INT
AS
BEGIN

	-- the summary of Standards for EPAO		
	SELECT DISTINCT EndPointAssessorOrganisationId,StandardCode
	INTO #LiveStandards
	FROM OrganisationStandard os1
	JOIN OrganisationStandardVersion osv ON osv.OrganisationStandardId = os1.Id
	WHERE os1.Status = 'Live' 
	AND (os1.EffectiveTo IS NULL OR os1.EffectiveTo >= GETDATE())
	AND osv.Status = 'Live' 
	AND (osv.EffectiveTo IS NULL OR osv.EffectiveTo >= GETDATE())
	AND EndPointAssessorOrganisationId = @epaOrgId
	;

	SELECT SUM(Standards) Standards, SUM(Pipeline) Pipeline, SUM(Assessments) Assessments
	FROM (
		-- The active records from ilr
		SELECT 0 Assessments, COUNT(*) Pipeline, 0 Standards
		FROM [dbo].[Learner] le1
		JOIN #LiveStandards lv1 on lv1.EndPointAssessorOrganisationId = le1.EpaOrgId AND lv1.StandardCode = le1.StdCode
		LEFT JOIN StandardCertificates ce1 on ce1.StandardCode = le1.StdCode AND ce1.Uln = le1.Uln
		WHERE ce1.Id IS NULL -- certificate exists
		-- and for continuing or recently completed Apprenticeships
		AND CompletionStatus IN (1,2)
		-- limit pipeline to completed, or continuing learning that has not yet lapsed
		AND (
			-- Learner has completed 
			CompletionStatus = 2 
			-- most recent activity (approval/ILR submission) is no more than 6(?) months ago
			OR (CompletionStatus = 1 AND LastUpdated >= DATEADD(month, -6, GETDATE()) )
			)
		-- limit Pipeline to the Estimated End Date is no more than the configurable pipeline cut off.
		AND EstimatedEndDate >= DATEADD(month, -@pipelineCutOff, GETDATE())
		AND EndPointAssessorOrganisationId = @epaOrgId
		-- 
		UNION ALL
		-- add in the created certificates (by epaOrgId)
		SELECT COUNT(*) Assessments, 0 Pipeline, 0 Standards
		FROM StandardCertificates ce2
		JOIN Organisations os2 ON ce2.OrganisationId = os2.Id
		WHERE ce2.[Status] NOT IN ('Deleted','Draft')
		AND EndPointAssessorOrganisationId = @epaOrgId
		--
		UNION ALL
		-- add in the org standards (by epaOrgId)
		SELECT 0 Assessments, 0 Pipeline, COUNT(*) Standards
		FROM #LiveStandards
	) [DashboardCounts];

	DROP TABLE  #LiveStandards;
	
END
GO