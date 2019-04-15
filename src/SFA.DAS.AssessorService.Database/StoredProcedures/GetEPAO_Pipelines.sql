CREATE PROCEDURE [GetEPAO_Pipelines]
    -- Add the parameters for the stored procedure here
      @EPAOID NVARCHAR(12)
AS
BEGIN    
-- check for all pipeline for EPAO 

WITH Data_CTE 
AS
(
    SELECT StdCode, Title, 
    COUNT(*) Pipeline, EstimateDate
    FROM (
    SELECT StdCode, CASE WHEN PlannedEndDate > GETDATE() THEN EOMONTH(PlannedEndDate) ELSE EOMONTH(DATEADD(month, std.Duration, LearnStartDate)) END AS EstimateDate , Title
    FROM (
    -- The active records from ilr
    SELECT Uln, StdCode, LearnStartDate, PlannedEndDate
    FROM ilrs il1
    JOIN OrganisationStandard os ON il1.EpaOrgId = os.EndPointAssessorOrganisationId AND il1.StdCode = os.StandardCode AND 
         os.[Status] <> 'Deleted' AND (os.[EffectiveTo] IS NULL OR os.[EffectiveTo] >= GETDATE())
    WHERE CompletionStatus IN (1,2) AND EpaOrgId = @EPAOID 
    ) il
    JOIN (SELECT standardid, title, CONVERT(numeric,JSON_VALUE([StandardData],'$.Duration')) Duration FROM [dbo].[StandardCollation]) std ON std.StandardId = il.StdCode
    -- ignore already created certs
    LEFT JOIN (SELECT DISTINCT uln, StandardCode FROM [Certificates] c1 ) ce ON ce.uln = il.uln AND ce.StandardCode = il.StdCode
    WHERE ce.uln IS NULL
    ) ab0
    GROUP BY StdCode,EstimateDate,title
), 
Count_CTE 
AS 
(
   SELECT COUNT(*) AS TotalRows FROM Data_CTE
)
SELECT *
FROM Data_CTE
CROSS JOIN Count_CTE
ORDER BY Title,EstimateDate
GO