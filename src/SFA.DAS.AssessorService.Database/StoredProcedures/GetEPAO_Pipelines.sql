CREATE PROCEDURE [dbo].[GetEPAO_Pipelines]
    -- Add the parameters for the stored procedure here
      @EPAOID NVARCHAR(12)
AS
BEGIN    
-- Apparently much more efficient then COUNT(*) OVER() or two seperate queries one for count and one for data
-- It does not create any temp table either, query cost is less when compared to other ways of getting count

WITH Data_CTE 
AS
(
    SELECT StdCode, Title, UkPrn,
    COUNT(*) Pipeline, EstimateDate
    FROM (
    SELECT StdCode, CASE WHEN PlannedEndDate > GETDATE() THEN EOMONTH(PlannedEndDate) ELSE EOMONTH(DATEADD(month, std.Duration, LearnStartDate)) END AS EstimateDate , Title, UkPrn
    FROM (
    -- The active records from ilr
    SELECT Uln, StdCode, UkPrn, EPAORGID, LearnStartDate, PlannedEndDate
    FROM ilrs il1
    JOIN OrganisationStandard os ON il1.EpaOrgId = os.EndPointAssessorOrganisationId AND il1.StdCode = os.StandardCode AND 
         os.[Status] <> 'Deleted' AND (os.[EffectiveTo] IS NULL OR os.[EffectiveTo] >= GETDATE())
    WHERE CompletionStatus IN (1,2) AND EpaOrgId = @EPAOID 
    ) il
    JOIN (SELECT standardid, title, CONVERT(numeric,JSON_VALUE([StandardData],'$.Duration')) Duration FROM [dbo].[StandardCollation]) std ON std.StandardId = il.StdCode
    -- ignore already created certs
    LEFT JOIN (SELECT uln, StandardCode, [EndPointAssessorOrganisationId] FROM [Certificates] c1 JOIN [Organisations] og on c1.[OrganisationId] = og.[Id] )
    ce ON ce.uln = il.uln AND ce.StandardCode = il.StdCode
    WHERE ce.EndPointAssessorOrganisationId IS NULL
    ) ab0
    GROUP BY StdCode,EstimateDate,title, UkPrn
    
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
END
GO