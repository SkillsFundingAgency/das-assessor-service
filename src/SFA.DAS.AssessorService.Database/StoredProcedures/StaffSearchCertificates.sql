CREATE PROCEDURE [dbo].[StaffSearchCertificates]
	 @Search nvarchar(50),
	 @Skip int,
	 @Take int
AS
BEGIN
DECLARE @SearchNoSpaces nvarchar(50) = REPLACE(@Search, ' ','')
SELECT 
REPLACE(JSON_VALUE(CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(CertificateData, '$.LearnerFamilyName'),' ','') AS x,
ce1.Uln,
LearnerFamilyName FamilyName, 
LearnerGivenNames GivenNames, 
ce1.StandardCode StdCode,
JSON_VALUE(CertificateData, '$.StandardName') AS StandardName,
ce1.CertificateReference, 
ce1.Status, 
JSON_VALUE(CertificateData, '$.LearningStartDate') LearnStartDate, 
org.[EndPointAssessorOrganisationId]  EPAOrgID,
learner1.FundingModel,
learner1.ApprenticeshipId,
learner1.Source,
ce1.CreatedAt,
ce1.UpdatedAt,
learner1.LearnRefNumber,
LearnerFamilyName AS CertFamilyName	
FROM Certificates ce1 
JOIN Organisations org ON ce1.OrganisationId = org.Id
LEFT JOIN Learner learner1 ON ce1.StandardCode = learner1.StdCode AND ce1.Uln = learner1.Uln
WHERE ce1.LearnerFamilyName = @Search
   OR ce1.LearnerGivenNames = @Search
   OR ce1.LearnerFullNameNoSpaces = @SearchNoSpaces 
UNION ALL
SELECT 
REPLACE(GivenNames,' ','') + REPLACE(FamilyName,' ','') AS x,
learner1.Uln ,
learner1.FamilyName, 
learner1.GivenNames, 
learner1.StdCode,
sc.Title AS StandardName,
NULL CertificateReference, 
NULL Status,
learner1.LearnStartDate,
learner1.EpaOrgId,
learner1.FundingModel,
learner1.ApprenticeshipId,
learner1.Source,
NULL CreatedAt,
NULL UpdatedAt,
learner1.LearnRefNumber,
NULL CertFamilyName	
FROM Learner learner1
JOIN Standards sc ON learner1.StdCode = sc.LarsCode
LEFT JOIN Certificates ce1 ON ce1.StandardCode = learner1.StdCode AND ce1.Uln = learner1.Uln
WHERE 
ce1.Uln IS  NULL
AND(learner1.FamilyName = @Search 
    OR learner1.GivenNames = @Search 
    OR learner1.LearnerFullNameNoSpaces = @SearchNoSpaces)  
ORDER BY x, CreatedAt
        OFFSET @Skip ROWS 
        FETCH NEXT @Take ROWS ONLY
END

