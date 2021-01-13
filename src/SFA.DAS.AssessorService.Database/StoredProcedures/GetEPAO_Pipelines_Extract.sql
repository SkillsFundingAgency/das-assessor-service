CREATE PROCEDURE [GetEPAO_Pipelines_Extract]
    @epaOrgId NVARCHAR(12)
AS
BEGIN
    WITH Data_CTE 
    AS
    (
        SELECT 
            StdCode, Title, UkPrn AS ProviderUkPrn, COUNT(*) Pipeline, EstimateDate
        FROM 
            [dbo].[EPAO_Func_Get_PipelineInfo] (@epaOrgId, NULL)
        GROUP BY 
            StdCode, EstimateDate, Title, Ukprn
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