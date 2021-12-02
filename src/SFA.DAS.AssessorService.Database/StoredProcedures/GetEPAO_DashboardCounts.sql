CREATE PROCEDURE [GetEPAO_DashboardCounts]
      @epaOrgId NVARCHAR(12),
	  @pipelineCutOff INT
AS
BEGIN
	SELECT SUM(Standards) Standards, SUM(Pipeline) Pipeline, SUM(Assessments) Assessments
	FROM (
		-- The active records from ilr
		SELECT 0 Assessments, COUNT(*) Pipeline, 0 Standards
		FROM [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, NULL, @pipelineCutOff)
		-- 
		UNION ALL
		-- add in the created certificates (by epaOrgId)
		SELECT COUNT(*) Assessments, 0 Pipeline, 0 Standards FROM Certificates ce2
		JOIN Organisations os2 ON ce2.OrganisationId = os2.Id AND EndPointAssessorOrganisationId = @epaOrgId
		WHERE ce2.[Status] NOT IN ('Deleted','Draft')
		--
		UNION ALL
		-- add in the org standards (by epaOrgId)
		SELECT 0 Assessments, 0 Pipeline, COUNT(*) Standards FROM OrganisationStandard os3
		WHERE [Status] <> 'Deleted' AND ([EffectiveTo] IS NULL OR [EffectiveTo] >= GETDATE()) AND EndPointAssessorOrganisationId = @epaOrgId
	) [DashboardCounts]
END
GO