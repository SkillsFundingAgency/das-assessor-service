CREATE PROCEDURE [GetEPAO_Pipelines_Count]
      @epaOrgId NVARCHAR(12),
      @stdCode int
AS
BEGIN 
    SELECT 
        COUNT(*) Pipeline
    FROM 
        [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, @stdCode)
END
GO