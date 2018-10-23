CREATE PROCEDURE [dbo].[StaffSearchCertificates]
	 @Search nvarchar(50),
	 @Skip int,
	 @Take int
AS
BEGIN
With IlrSearch As
(
	SELECT * FROM (
	SELECT    c.IsPrivatelyFunded,
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
	WHERE ((REPLACE(FamilyName, ' ','') = @Search 
		OR REPLACE(GivenNames, ' ','') = @Search 
		OR REPLACE(GivenNames, ' ','') + REPLACE(FamilyName, ' ','') = REPLACE(@Search, ' ',''))
		OR (
			REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerFamilyName'),' ','') = @Search
			OR REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerGivenNames'),' ','') = @Search
			OR REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerFamilyName'),' ','') = REPLACE(@Search, ' ','')
		))
		 
		 AND  ((1=(CASE WHEN c.IsPrivatelyFunded IS NULL THEN 1 ELSE 0 END) OR (c.IsPrivatelyFunded = 0))
		 ))
					
		AS Learners
			ORDER BY CertificateReference
			OFFSET @Skip ROWS 
			FETCH NEXT @Take ROWS ONLY
		),
		PrivateCertificateSearch AS
		(
			SELECT * FROM (
			SELECT    c.IsPrivatelyFunded,
			REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerFamilyName'),' ','') AS x,
			c.Uln, 
				CASE WHEN JSON_VALUE(c.CertificateData, '$.LearnerGivenNames') IS NULL THEN NULL ELSE  JSON_VALUE(c.CertificateData, '$.LearnerGivenNames') END AS GivenNames,
				CASE WHEN JSON_VALUE(c.CertificateData, '$.LearnerFamilyName') IS NULL THEN NULL
				 ELSE  JSON_VALUE(c.CertificateData, '$.LearnerFamilyName') END AS FamilyName,
				c.StandardCode, 
				JSON_VALUE(c.CertificateData, '$.StandardName') AS Standard, 
				c.CertificateReference, 
				c.Status, 
				NULL AS LearnStartDate,
				NULL AS EpaOrgId,
				NULL AS FundingModel,
				NULL AS ApprenticeshipId,
				NULL AS EmployerAccountId,
				NULL AS Source,
				c.CreatedAt AS CreatedAt,
				c.UpdatedAt,
				c.LearnRefNumber,
				JSON_VALUE(c.CertificateData, '$.LearnerFamilyName') AS CertFamilyName
			FROM Certificates c	
			INNER JOIN Organisations o ON
				c.OrganisationId = o.Id
			WHERE ((
					REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerFamilyName'),' ','') = @Search
					OR REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerGivenNames'),' ','') = @Search
					OR REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerGivenNames'),' ','') + REPLACE(JSON_VALUE(c.CertificateData, '$.LearnerFamilyName'),' ','') = REPLACE(@Search, ' ','')
				))
		 
				AND  (c.IsPrivatelyFunded = 1))
				AS Learners
					ORDER BY CertificateReference
					OFFSET @Skip ROWS 
					FETCH NEXT @Take ROWS ONLY
				)
		SELECT * FROM IlrSearch
		UNION ALL
		SELECT  *
		FROM    PrivateCertificateSearch
END