CREATE PROCEDURE [dbo].[StaffSearchCertificates]
	 @Search nvarchar(50),
	 @Skip int,
	 @Take int
AS
BEGIN
SELECT 
REPLACE(JSON_VALUE(CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(CertificateData, '$.LearnerFamilyName'),' ','') AS x,
ce1.Uln,
JSON_VALUE(CertificateData, '$.LearnerFamilyName') FamilyName, 
JSON_VALUE(CertificateData, '$.LearnerGivenNames') GivenNames, 
ce1.StandardCode StdCode,
JSON_VALUE(CertificateData, '$.StandardName') AS StandardName,
ce1.CertificateReference, 
ce1.Status, 
JSON_VALUE(CertificateData, '$.LearningStartDate') LearnStartDate, 
org.[EndPointAssessorOrganisationId]  EPAOrgID,
il1.FundingModel,
il1.ApprenticeshipId,
il1.EmployerAccountId,
il1.Source,
ce1.CreatedAt,
ce1.UpdatedAt,
il1.EventId,
il1.LearnRefNumber,
JSON_VALUE(CertificateData, '$.LearnerFamilyName') AS CertFamilyName	
FROM Certificates ce1 
JOIN Organisations org ON ce1.OrganisationId = org.Id
LEFT JOIN Ilrs il1 ON ce1.StandardCode = il1.StdCode AND ce1.Uln = il1.Uln
WHERE JSON_VALUE(CertificateData, '$.LearnerFamilyName') = @Search
   OR JSON_VALUE(CertificateData, '$.LearnerGivenNames') = @Search
   OR REPLACE(JSON_VALUE(CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(CertificateData, '$.LearnerFamilyName'),' ','') = REPLACE(@Search, ' ','') 
UNION ALL
SELECT 
REPLACE(GivenNames,' ','') + REPLACE(FamilyName,' ','') AS x,
il1.Uln ,
il1.FamilyName, 
il1.GivenNames, 
il1.StdCode,
sc.Title AS StandardName,
NULL CertificateReference, 
NULL Status,
il1.LearnStartDate,
il1.EpaOrgId,
il1.FundingModel,
il1.ApprenticeshipId,
il1.EmployerAccountId,
il1.Source,
il1.CreatedAt,
il1.UpdatedAt,
il1.EventId,
il1.LearnRefNumber,
NULL CertFamilyName	
FROM Ilrs il1
JOIN StandardCollation sc ON il1.StdCode = sc.StandardId
LEFT JOIN Certificates ce1 ON ce1.StandardCode = il1.StdCode AND ce1.Uln = il1.Uln
WHERE 
ce1.Uln IS  NULL
AND(FamilyName = @Search 
    OR GivenNames = @Search 
    OR REPLACE(GivenNames, ' ','') + REPLACE(FamilyName, ' ','') =  REPLACE(@Search, ' ','') 	)  
ORDER BY x, CreatedAt
        OFFSET @Skip ROWS 
        FETCH NEXT @Take ROWS ONLY
END

