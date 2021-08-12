CREATE PROCEDURE [GetEPAO_Pipelines]
    @epaOrgId NVARCHAR(12),
    @standardFilterId NVARCHAR(12),
    @providerFilterId NVARCHAR(12),
    @epaDateFilterId NVARCHAR(12)
AS
BEGIN
    WITH Data_CTE 
    AS
    (
        SELECT 
            StdCode, Title, Version, UkPrn, ProviderName, COUNT(*) Pipeline, EstimateDate
        FROM 
            [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, NULL)
        GROUP BY 
            StdCode, UkPrn, ProviderName, EstimateDate, Title, Version
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
    WHERE
        (StdCode = @standardFilterId OR @standardFilterId IS NULL) AND (UkPrn = @providerFilterId OR @providerFilterId IS NULL)  AND (FORMAT(EstimateDate, 'yyyyMM') = @epaDateFilterId OR @epaDateFilterId IS NULL)
    ORDER BY 
        Title, EstimateDate
END
GO