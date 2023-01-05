/*
	Update ApplyViaOptInStatus
*/

MERGE INTO Apply Ap1
USING
(
SELECT Apply.Id, ApplyViaOptIn, Apply.CreatedAt, JSON_QUERY(Apply.ApplyData, '$.Apply.Versions') ApplyVersions, Apply.StandardApplicationType, A.[StandardUId]
    ,A.[Version]
    ,A.[OrganisationStandardId]
    ,A.[Comments]
    ,A.[Status]
	,B.[EndPointAssessorOrganisationId]
	,B.[StandardReference]
	,Apply.OrganisationId
FROM [OrganisationStandardVersion] A
JOIN [OrganisationStandard] B ON A.OrganisationStandardId = B.Id
JOIN [Organisations] C ON B.[EndPointAssessorOrganisationId] = C.[EndPointAssessorOrganisationId]
JOIN Apply ON Apply.OrganisationId = C.Id AND Apply.StandardReference = B.StandardReference 
WHERE A.Comments Like '%Opted%'  
AND ISNULL(Apply.StandardApplicationType,'') NOT LIKE '%Withdrawal%' 
AND ISNULL(ApplyViaOptIn,0) != 1
AND JSON_QUERY(Apply.ApplyData, '$.Apply.Versions') LIKE '%"' +  A.[Version] + '"%'
) upd
ON Ap1.Id = upd.Id
WHEN MATCHED THEN UPDATE SET Ap1.ApplyViaOptin = 1;
