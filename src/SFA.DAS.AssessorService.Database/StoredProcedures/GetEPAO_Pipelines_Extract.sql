CREATE PROCEDURE [GetEPAO_Pipelines_Extract]
    @epaOrgId NVARCHAR(12),
    @standardFilterId NVARCHAR(12),
    @providerFilterId NVARCHAR(12),
    @epaDateFilterId NVARCHAR(6),
    @pipelineCutOff INT
AS
BEGIN
    WITH Data_CTE 
    AS
    (
        SELECT 
            StdCode, Title, Version, UkPrn AS ProviderUkPrn, ProviderName, COUNT(*) Pipeline, EstimateDate
        FROM 
            [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, NULL, @pipelineCutOff)
        WHERE
			(StdCode = @standardFilterId OR @standardFilterId IS NULL) AND (UkPrn = @providerFilterId OR @providerFilterId IS NULL)  AND (FORMAT(EstimateDate, 'yyyyMM') = @epaDateFilterId OR @epaDateFilterId IS NULL)
        GROUP BY 
            StdCode, Version, EstimateDate, Title, Ukprn, ProviderName
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