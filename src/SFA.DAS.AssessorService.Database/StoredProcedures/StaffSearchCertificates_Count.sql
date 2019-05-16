CREATE PROCEDURE [dbo].[StaffSearchCertificates_Count]
	@Search nvarchar(50)
AS
BEGIN
	SELECT SUM(ab.Count)
	FROM
	(
		SELECT COUNT(ce1.Id) AS 'Count'
		FROM Certificates ce1 
		JOIN Organisations org ON ce1.OrganisationId = org.id
		LEFT JOIN Ilrs il1 ON ce1.standardcode = il1.stdcode AND ce1.uln = il1.uln
		WHERE JSON_VALUE(CertificateData, '$.LearnerFamilyName') = @Search
		   OR JSON_VALUE(CertificateData, '$.LearnerGivenNames') = @Search
		   OR REPLACE(JSON_VALUE(CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(CertificateData, '$.LearnerFamilyName'),' ','') = REPLACE(@Search, ' ','') 
		UNION ALL 
		SELECT COUNT(il1.Id) AS 'Count'
		FROM Ilrs il1
		JOIN StandardCollation sc ON il1.StdCode = sc.StandardId
		LEFT JOIN Certificates ce1 ON ce1.standardcode = il1.stdcode AND ce1.uln = il1.uln
		WHERE 
		ce1.uln IS  NULL
		AND(FamilyName = @Search 
			OR GivenNames = @Search 
			OR REPLACE(GivenNames, ' ','') + REPLACE(FamilyName, ' ','') =  REPLACE(@Search, ' ','') 	)  
	) as ab
END