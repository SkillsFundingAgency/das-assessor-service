CREATE PROCEDURE [GetEPAO_Pipelines_Count]
      @epaOrgId NVARCHAR(12),
      @stdCode int,
      @pipelineCutOff INT
AS
BEGIN 
    SELECT 
        COUNT(*) Pipeline
    FROM 
        [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, @stdCode, @pipelineCutOff)
END
GO