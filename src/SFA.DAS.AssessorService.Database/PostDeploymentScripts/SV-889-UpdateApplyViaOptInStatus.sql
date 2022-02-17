/*
	Update ApplyViaOptInStatus
*/

UPDATE Apply SET 
ApplyViaOptIn = 1
WHERE EXISTS 
(
  SELECT A.[StandardUId]
        ,A.[Version]
        ,A.[OrganisationStandardId]
        ,A.[Comments]
        ,A.[Status]
	    ,B.[EndPointAssessorOrganisationId]
		,B.[StandardReference]
	    ,C.Id
  FROM [OrganisationStandardVersion] A
	  LEFT JOIN [OrganisationStandard] B ON
		A.OrganisationStandardId = B.Id
	  LEFT JOIN [Organisations] C ON
		B.[EndPointAssessorOrganisationId] = C.[EndPointAssessorOrganisationId]
  WHERE A.Comments Like '%Opt%' AND 
  Apply.OrganisationId = C.Id AND
  Apply.StandardReference = B.StandardReference AND
  Apply.StandardApplicationType NOT LIKE '%Withdrawal%' AND
  JSON_QUERY(Apply.ApplyData, '$.Apply.Versions') LIKE '%' +  A.[Version] + '%'
)
