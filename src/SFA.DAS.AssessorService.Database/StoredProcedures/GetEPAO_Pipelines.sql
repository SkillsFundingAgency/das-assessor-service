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
    SELECT StdCode,
	COUNT(*) Pipeline, EstimateDate
	FROM (
	SELECT StdCode, EOMONTH(DATEADD(month, std.Duration, LearnStartDate)) AS EstimateDate 
	FROM (
	-- The most recent (as far as we can tell) record from ilr
	SELECT * FROM (
	SELECT Uln, StdCode, FamilyName, EPAORGID, completionStatus, row_number() OVER (PARTITION BY Uln, StdCode ORDER BY source DESC, learnstartdate desc) rownumber, 
	[LearnStartDate]
	FROM ilrs
	) ab1 WHERE ab1.rownumber = 1 AND ab1.CompletionStatus IN (1,2) 
	AND EpaOrgId = @EPAOID
	) il
	JOIN (SELECT standardid, CONVERT(numeric,JSON_VALUE([StandardData],'$.Duration')) Duration FROM [dbo].[StandardCollation]) std ON std.StandardId = il.StdCode
	-- ignore already created certs
	LEFT JOIN (SELECT uln, StandardCode, [EndPointAssessorOrganisationId] FROM [Certificates] c1 JOIN [Organisations] og on c1.[OrganisationId] = og.[Id] )  
	ce ON ce.uln = il.uln AND ce.StandardCode = il.StdCode
	WHERE ce.EndPointAssessorOrganisationId IS NULL
	) ab0
	GROUP BY StdCode,EstimateDate
	
), 
Count_CTE 
AS 
(
   SELECT COUNT(*) AS TotalRows FROM Data_CTE
)
SELECT *
FROM Data_CTE
CROSS JOIN Count_CTE
ORDER BY 2,5
END
GO

