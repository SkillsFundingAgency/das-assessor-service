CREATE PROCEDURE [dbo].[GetEPAO_Pipelines]
	-- Add the parameters for the stored procedure here
	  @EPAOID NVARCHAR(12)	
AS
BEGIN	

	SELECT StdCode, COUNT(*) Pipeline, EstimateDate
	FROM (
		SELECT StdCode, EOMONTH(DATEADD(month, std.Duration, LearnStartDate)) AS EstimateDate 
		FROM (
			-- The most recent (as far as we can tell) record from ilr
				SELECT * FROM (
					SELECT Uln, StdCode, FamilyName, EPAORGID, completionStatus, row_number() OVER (PARTITION BY Uln, StdCode, FamilyName ORDER BY source DESC, learnstartdate desc) rownumber, 
					[LearnStartDate]
					FROM ilrs
				) ab1 WHERE ab1.rownumber = 1 AND ab1.CompletionStatus IN (1,2) AND EpaOrgId = @EPAOID
			) il JOIN (
					SELECT standardid, CONVERT(NUMERIC,JSON_VALUE([StandardData],'$.Duration')
			 ) Duration FROM [dbo].[StandardCollation]
		) std ON std.StandardId = il.StdCode
		-- ignore already created certs
		LEFT JOIN (
			SELECT uln, StandardCode, [EndPointAssessorOrganisationId] FROM [Certificates] c1 JOIN [Organisations] og on c1.[OrganisationId] = og.[Id] ) ce ON ce.uln = il.uln AND ce.StandardCode = il.StdCode
			WHERE ce.EndPointAssessorOrganisationId IS NULL
			) ab0
	GROUP BY StdCode,EstimateDate
	ORDER BY 1,3
RETURN 0
END
GO
