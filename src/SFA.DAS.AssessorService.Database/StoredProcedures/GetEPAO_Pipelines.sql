CREATE PROCEDURE [GetEPAO_Pipelines]
    -- Add the parameters for the stored procedure here
      @EPAOID NVARCHAR(12)
AS
BEGIN    
-- check for all pipeline for EPAO (NOTE use of COUNT(*) OVER () AS TotalRows )

SELECT StdCode, Title, 
    COUNT(*) Pipeline, EstimateDate, COUNT(*) OVER () AS TotalRows
FROM (
    SELECT StdCode, CASE WHEN PlannedEndDate > GETDATE() THEN EOMONTH(PlannedEndDate) ELSE EOMONTH(DATEADD(month, std.Duration, LearnStartDate)) END AS EstimateDate , Title
    FROM (
    -- The active records from ilr
    SELECT Uln, StdCode, EPAORGID, LearnStartDate, PlannedEndDate
    FROM ilrs il1
    JOIN OrganisationStandard os ON il1.EpaOrgId = os.EndPointAssessorOrganisationId AND il1.StdCode = os.StandardCode AND 
         os.[Status] <> 'Deleted' AND (os.[EffectiveTo] IS NULL OR os.[EffectiveTo] >= GETDATE())
    WHERE CompletionStatus IN (1,2) AND EpaOrgId = @EPAOID  
    ) il
    JOIN (SELECT standardid, title, CONVERT(numeric,JSON_VALUE([StandardData],'$.Duration')) Duration FROM [StandardCollation]) std ON std.StandardId = il.StdCode
    -- ignore already created certs
    LEFT JOIN (SELECT DISTINCT uln, StandardCode, [EndPointAssessorOrganisationId] FROM [Certificates] c1 JOIN [Organisations] og on c1.[OrganisationId] = og.[Id])
    ce ON ce.uln = il.uln AND ce.StandardCode = il.StdCode
    WHERE ce.EndPointAssessorOrganisationId IS NULL
) ab0
GROUP BY StdCode,EstimateDate,title
ORDER BY Title, EstimateDate
END
GO