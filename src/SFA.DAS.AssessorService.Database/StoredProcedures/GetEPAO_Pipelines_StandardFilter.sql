CREATE PROCEDURE [GetEPAO_Pipelines_StandardFilter]
      @epaOrgId NVARCHAR(12),
      @pipelineCutOff INT
AS
BEGIN 
    SELECT 
        DISTINCT StdCode AS [Id], Title AS [Value]
    FROM 
        [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, NULL, @pipelineCutOff)
    ORDER BY Value
END
GO