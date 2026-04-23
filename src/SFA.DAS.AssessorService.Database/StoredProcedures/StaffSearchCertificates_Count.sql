CREATE PROCEDURE [dbo].[StaffSearchCertificates_Count]
	@Search nvarchar(50)
AS
BEGIN
	DECLARE @SearchNoSpaces nvarchar(50) = REPLACE(@Search, ' ','')
	SELECT COUNT(DISTINCT ab.Course)
	FROM
	(
		SELECT ce1.Uln+CONVERT(varchar,ce1.StandardCode) AS Course
		FROM StandardCertificates ce1 
		WHERE ce1.LearnerFamilyName = @Search 
		OR ce1.LearnerGivenNames = @Search 
		OR ce1.LearnerFullNameNoSpaces = @SearchNoSpaces
		UNION ALL 
		SELECT learner1.Uln+CONVERT(varchar,learner1.StdCode) AS Course
		FROM Learner learner1
		LEFT JOIN StandardCertificates ce1 ON ce1.StandardCode = learner1.StdCode AND ce1.Uln = learner1.Uln
		WHERE ce1.Id IS NULL
		AND (learner1.FamilyName = @Search
		  OR learner1.GivenNames = @Search
		  OR learner1.LearnerFullNameNoSpaces = @SearchNoSpaces)
	) as ab
END