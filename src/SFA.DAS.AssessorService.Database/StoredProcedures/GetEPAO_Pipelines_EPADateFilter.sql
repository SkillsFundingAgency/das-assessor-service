CREATE PROCEDURE [GetEPAO_Pipelines_EPADateFilter]
      @epaOrgId NVARCHAR(12)
AS
BEGIN 
    SELECT 
        DISTINCT FORMAT(EstimateDate, 'yyyy-MM-dd') as [Id], EstimateDate as [Value]
    FROM 
        [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, NULL)
    ORDER BY Id DESC
END
GO