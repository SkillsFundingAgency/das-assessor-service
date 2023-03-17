CREATE PROCEDURE [GetEPAO_Pipelines_ProviderFilter]
      @epaOrgId NVARCHAR(12),
      @pipelineCutOff INT
AS
BEGIN 
    SELECT 
        DISTINCT Ukprn AS [Id], ProviderName AS [Value]
    FROM 
        [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, NULL, @pipelineCutOff)
    ORDER BY ProviderName
END
GO