CREATE PROCEDURE [dbo].[StaffSearchCertificates_Count]
	@Search nvarchar(50)
AS
BEGIN
	DECLARE @SearchNoSpaces nvarchar(50) = REPLACE(@Search, ' ','')
	SELECT SUM(ab.Count)
	FROM
	(
		SELECT COUNT(ce1.Id) AS 'Count'
		FROM StandardCertificates ce1 
		JOIN Organisations org ON ce1.OrganisationId = org.Id
		LEFT JOIN Learner learner1 ON ce1.StandardCode = learner1.StdCode AND ce1.Uln = learner1.Uln
		WHERE ce1.LearnerFamilyName = @Search 
		OR ce1.LearnerGivenNames = @Search 
		OR ce1.LearnerFullNameNoSpaces = @SearchNoSpaces
		UNION ALL 
		SELECT COUNT(learner1.Id) AS 'Count'
		FROM Learner learner1
		JOIN Standards sc ON learner1.StdCode = sc.LarsCode
		LEFT JOIN StandardCertificates ce1 ON ce1.StandardCode = learner1.StdCode AND ce1.Uln = learner1.Uln
		WHERE 
		ce1.Uln IS NULL AND
		(learner1.FamilyName = @Search OR learner1.GivenNames = @Search  OR learner1.LearnerFullNameNoSpaces =  @SearchNoSpaces)  
	) as ab
END