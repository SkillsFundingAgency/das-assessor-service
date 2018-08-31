CREATE PROCEDURE [dbo].[StaffSearchCertificates]
	 @Search nvarchar(50),
	 @Skip int,
	 @Take int
AS
BEGIN
SELECT * FROM (
SELECT    
REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerFamilyName'),' ','') AS x,
i.Uln, 
    CASE WHEN JSON_VALUE(c.CertificateData, '$.LearnerGivenNames') IS NULL THEN i.GivenNames ELSE  JSON_VALUE(c.CertificateData, '$.LearnerGivenNames') END AS GivenNames,
    CASE WHEN JSON_VALUE(c.CertificateData, '$.LearnerFamilyName') IS NULL THEN i.FamilyName ELSE  JSON_VALUE(c.CertificateData, '$.LearnerFamilyName') END AS FamilyName,
    i.StdCode, 
	JSON_VALUE(c.CertificateData, '$.StandardName') AS Standard, 
	c.CertificateReference, 
	c.Status, 
	i.LearnStartDate,
	i.EpaOrgId,
	i.FundingModel,
	i.ApprenticeshipId,
	i.EmployerAccountId,
	i.Source,
	i.CreatedAt AS CreatedAt,
	i.UpdatedAt,
	i.LearnRefNumber,
    JSON_VALUE(c.CertificateData, '$.LearnerFamilyName') AS CertFamilyName
FROM Ilrs i
LEFT OUTER JOIN Certificates c ON c.Uln = i.Uln AND c.StandardCode = i.StdCode
WHERE (REPLACE(FamilyName, ' ','') = @Search 
    OR REPLACE(GivenNames, ' ','') = @Search 
    OR REPLACE(GivenNames, ' ','') + REPLACE(FamilyName, ' ','') = REPLACE(@Search, ' ',''))
    OR (
        REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerFamilyName'),' ','') = @Search
        OR REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerGivenNames'),' ','') = @Search
        OR REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerFamilyName'),' ','') = REPLACE(@Search, ' ','')
        )
        ) AS Learners
        ORDER BY FamilyName, GivenNames
        OFFSET @Skip ROWS 
        FETCH NEXT @Take ROWS ONLY
END