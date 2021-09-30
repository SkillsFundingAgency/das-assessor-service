CREATE PROCEDURE [dbo].[StaffSearchCertificates_Count]
	@Search nvarchar(50)
AS
BEGIN
	SELECT SUM(ab.Count)
	FROM
	(
		SELECT COUNT(ce1.Id) AS 'Count'
		FROM Certificates ce1 
		JOIN Organisations org ON ce1.OrganisationId = org.Id
		LEFT JOIN Learner learner1 ON ce1.StandardCode = learner1.StdCode AND ce1.Uln = learner1.Uln
		WHERE JSON_VALUE(CertificateData, '$.LearnerFamilyName') = @Search
		   OR JSON_VALUE(CertificateData, '$.LearnerGivenNames') = @Search
		   OR REPLACE(JSON_VALUE(CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(CertificateData, '$.LearnerFamilyName'),' ','') = REPLACE(@Search, ' ','') 
		UNION ALL 
		SELECT COUNT(learner1.Id) AS 'Count'
		FROM Learner learner1
		JOIN StandardCollation sc ON learner1.StdCode = sc.StandardId
		LEFT JOIN Certificates ce1 ON ce1.StandardCode = learner1.StdCode AND ce1.Uln = learner1.Uln
		WHERE 
		ce1.Uln IS  NULL
		AND(FamilyName = @Search 
			OR GivenNames = @Search 
			OR REPLACE(GivenNames, ' ','') + REPLACE(FamilyName, ' ','') =  REPLACE(@Search, ' ','') 	)  
	) as ab
END