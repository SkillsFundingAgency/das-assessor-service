CREATE PROCEDURE [GetEPAO_Pipelines_ProviderFilter]
      @epaOrgId NVARCHAR(12)
AS
BEGIN 
    SELECT 
        DISTINCT UkPrn AS [Id], ProviderName AS [Value]
    FROM 
        [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, NULL)
    ORDER BY ProviderName
END
GO