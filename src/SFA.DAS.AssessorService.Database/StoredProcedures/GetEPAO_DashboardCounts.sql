CREATE PROCEDURE [GetEPAO_DashboardCounts]
    -- Add the parameters for the stored procedure here
      @EPAOID NVARCHAR(12)
AS
BEGIN    
-- check for all pipeline for EPAO 

-- the dashboard counts
SELECT SUM(Standards) Standards, SUM(Pipeline) Pipeline, SUM(Assessments) Assessments
FROM (
    -- The active records from ilr
    SELECT 0 Assessments, COUNT(*) Pipeline, 0 Standards
    FROM ilrs il
    JOIN (SELECT standardid, title, CONVERT(numeric,JSON_VALUE([StandardData],'$.Duration')) Duration FROM [dbo].[StandardCollation]) std ON std.StandardId = il.StdCode
    -- ignore already created certificates (by uln and standardcode)
    LEFT JOIN (SELECT DISTINCT uln, StandardCode FROM [Certificates] c1 ) ce ON ce.uln = il.uln AND ce.StandardCode = il.StdCode
    WHERE CompletionStatus IN (1,2) AND EpaOrgId = @EPAOID
	-- limit Pipeline to where Ilr is completed or most recent active submission is no more than 6 months ago
	AND ( (FundingModel = 36 and CompletionStatus = 1 AND ISNULL(il.UpdatedAt,il.createdAt) >= DATEADD(month,-6,GETDATE()) ) OR CompletionStatus = 2 OR FundingModel != 36) 
	-- limit Pipeline to where the Planned End Date (or Estimated End Date) is no more than 3 months in the past.
  	AND (CASE WHEN PlannedEndDate > GETDATE() THEN EOMONTH(PlannedEndDate) ELSE EOMONTH(DATEADD(month, std.Duration, LearnStartDate)) END) >= DATEADD(month,-3,GETDATE())
    AND ce.uln IS NULL
	-- 
	UNION ALL
	-- add in the created certificates (by epaorgid)
	SELECT COUNT(*) Assessments, 0 Pipeline, 0 Standards FROM Certificates ce2
	JOIN organisations os2 ON ce2.OrganisationId = os2.Id AND EndPointAssessorOrganisationId = @EPAOID
	WHERE ce2.[Status] NOT IN ('Deleted','Draft')
	--
	UNION ALL
	-- add in the org standards (by epaorgid)
	SELECT 0 Assessments, 0 Pipeline, COUNT(*) Standards FROM OrganisationStandard os3
	WHERE [Status] <> 'Deleted' AND ([EffectiveTo] IS NULL OR [EffectiveTo] >= GETDATE()) AND EndPointAssessorOrganisationId = @EPAOID
) ab1

END
GO