CREATE PROCEDURE [GetEPAO_Pipelines]
    @epaOrgId NVARCHAR(12),
    @pipelineCutOff INT
AS
BEGIN
    WITH Data_CTE 
    AS
    (
        SELECT 
            StdCode, Title, COUNT(*) Pipeline, EstimateDate
        FROM 
            [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, NULL, @pipelineCutOff)
        GROUP BY 
            StdCode, EstimateDate, Title
    ), 
    Count_CTE 
    AS 
    (
       SELECT COUNT(*) AS TotalRows FROM Data_CTE
    )
    
    SELECT 
        *
    FROM 
        Data_CTE
        CROSS JOIN Count_CTE
    ORDER BY 
        Title, EstimateDate
END
GO