-- To check Certificate and learner states
/*
STATE
1. Certificate Submitted
2. EPA Fail
3. EPA Pass
4. Draft Certificate (in UI)
5. Draft Certificate (in API)
6. Previous Fail now back in Draft Certificate in UI
7. Deleted Certificate
8. No EPA / Certificate
9. No match
possible also bad data - which shouldn't happen!

*/

-- !!! set this to see the data from the perspective of this EPAO !!!
DECLARE @epaorgid varchar(100) = 'EPA0008';

WITH WhoCanSeeCertificate AS
(SELECT @epaorgid Chosen_EPAO, Find_EPAO,
CASE WHEN Find_EPAO = 0 AND EPAO_restrictions NOT IN (5,4) THEN 'No match' 
     WHEN Find_EPAO = 1 AND EPAO_restrictions in (3) AND qa2.certificateStatus IN ('Draft','Deleted') THEN 'EPAO cannot see '+qa2.certificateStatus +' certificate record'
	 ELSE CASE EPAO_restrictions
	    	WHEN 5 THEN 'EPAO is creator and approved'
			WHEN 4 THEN 'EPAO is creator, no longer approved'
			WHEN 3 THEN 'EPAO can see redacted certificate'
			WHEN 2 THEN 'Other EPAO is creator #1'
			WHEN 1 THEN 'Other EPAO is creator #2'
			WHEN 0 THEN 'Other EPAO is creator #3'
			ELSE 'Not Approved'
		END
	 END What_is_allowed_for_Certificate
, EPAO_restrictions,
qa2.EndPointAssessorOrganisationId, qa2.larscode, qa2.certificatereference, qa2.uln, qa2.certificateStatus
FROM (
SELECT 
MAX ( CASE WHEN os2.EndPointAssessorOrganisationId = @epaorgid  THEN
	   (CASE WHEN qa1.EndPointAssessorOrganisationId = os2.EndPointAssessorOrganisationId AND ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN  5 -- chosen EPAO is creator and approved
             WHEN qa1.EndPointAssessorOrganisationId = os2.EndPointAssessorOrganisationId AND NOT ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN  4 -- chosen EPAO is creator, no longer approved
			 WHEN qa1.EndPointAssessorOrganisationId != os2.EndPointAssessorOrganisationId THEN 3 -- chosen EPAO can see redacted certificate
			 ELSE NULL
			 END )
		ELSE
	   (CASE WHEN qa1.EndPointAssessorOrganisationId = os2.EndPointAssessorOrganisationId AND ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN  2 -- other EPAO is creator and approved
             WHEN qa1.EndPointAssessorOrganisationId = os2.EndPointAssessorOrganisationId AND NOT ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN  1 -- other EPAO is creator, no longer approved
			 WHEN qa1.EndPointAssessorOrganisationId != os2.EndPointAssessorOrganisationId THEN 0 -- creator was never approved (?) /other EPAO can see redacted certificate
			 ELSE NULL
			 END )
END ) EPAO_restrictions
,qa1.EndPointAssessorOrganisationId, qa1.larscode, qa1.certificatereference, qa1.uln, qa1.certificateStatus,
(SELECT COUNT(*) FROM OrganisationStandard WHERE EndPointAssessorOrganisationId = @epaorgid AND standardcode = qa1.larscode) Find_EPAO
FROM 
( SELECT EndPointAssessorOrganisationId, standardcode larscode,certificatereference, Uln, ce2.status certificateStatus FROM certificates ce2 JOIN Organisations og2 on og2.Id = ce2.OrganisationId ) qa1
  LEFT JOIN OrganisationStandard os2
ON qa1.larscode = os2.standardcode
WHERE 1=1 
--and qa1.uln in ( 6417214655, 1009988667 )
GROUP BY qa1.EndPointAssessorOrganisationId, qa1.larscode, qa1.certificatereference, qa1.uln, qa1.certificateStatus
) qa2 
)  -- end of WhoCanSeeCertificate
,
WhoCanSeeILR AS
(
SELECT @epaorgid Chosen_EPAO, Find_EPAO,
CASE WHEN Find_EPAO = 0 THEN 'No match' 
	 ELSE CASE EPAO_restrictions
	    	WHEN 7 THEN 'EPAOrgId is in ILR and is approved'
			WHEN 6 THEN 'EPAOrgId not in ILR, can request certificate'
			WHEN 5 THEN 'Other EPAOrgId is in ILR, can request certificate'
			WHEN 4 THEN 'Other EPAOrgId is not in ILR, can request certificate'
			WHEN 3 THEN 'EPAOrgId in ILR is no longer approved'
			WHEN 2 THEN 'EPAOrgId is not in ILR, and is no longer approved'
			WHEN 1 THEN 'Other EPAOrgId is in ILR, but is no longer approved'
			WHEN 0 THEN 'Other EPAOrgId is not in ILR, and is no longer approved'
		END
	 END What_is_allowed_for_ILR
, EPAO_restrictions,
it2.EndPointAssessorOrganisationId, it2.larscode, it2.Uln
FROM (
SELECT 
MAX ( CASE WHEN os2.EndPointAssessorOrganisationId = @epaorgid  THEN
	   (CASE WHEN it1.EndPointAssessorOrganisationId = os2.EndPointAssessorOrganisationId AND ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN  7 -- chosen EPAO is in ILR and is approved
             WHEN it1.EndPointAssessorOrganisationId = os2.EndPointAssessorOrganisationId AND NOT ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN  3 -- chosen EPAO is in ILR but is no longer approved, cannot request
			 WHEN it1.EndPointAssessorOrganisationId != os2.EndPointAssessorOrganisationId AND ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN 6 -- chosen EPAO not in ILR, but can request certificate
             WHEN it1.EndPointAssessorOrganisationId != os2.EndPointAssessorOrganisationId AND NOT ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN 2 -- chosen EPAO is not in ILR, and is no longer approved, cannot request
			 ELSE NULL
			 END )
		ELSE
	   (CASE WHEN it1.EndPointAssessorOrganisationId = os2.EndPointAssessorOrganisationId AND ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN  5 -- other EPAO is in ILR, and can request certificate
             WHEN it1.EndPointAssessorOrganisationId = os2.EndPointAssessorOrganisationId AND NOT ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN  1 -- other EPAO is in ILR, and no longer approved, cannot request
			 WHEN it1.EndPointAssessorOrganisationId != os2.EndPointAssessorOrganisationId AND ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN 4 -- other EPAO not in ILR but is approved and can request certificate
			 WHEN it1.EndPointAssessorOrganisationId != os2.EndPointAssessorOrganisationId AND NOT ( os2.EffectiveTo IS NULL OR os2.EffectiveTo >= GETDATE()) THEN 0 -- other EPAO not in ILR and is no longer approved, cannot request certificate
			 ELSE NULL
			 END )
END ) EPAO_restrictions
,it1.EndPointAssessorOrganisationId, it1.larscode, it1.uln,
(SELECT COUNT(*) FROM OrganisationStandard WHERE EndPointAssessorOrganisationId = @epaorgid AND standardcode = it1.larscode) Find_EPAO
FROM 
( SELECT EPAOrgId EndPointAssessorOrganisationId, StdCode larscode,uln FROM ilrs il2 JOIN Organisations og2 on og2.EndPointAssessorOrganisationId = il2.EpaOrgId ) it1
  JOIN OrganisationStandard os2
ON it1.larscode = os2.standardcode
WHERE 1=1
GROUP BY it1.EndPointAssessorOrganisationId, it1.larscode, it1.uln
) it2 
) -- end of WhoCanSeeILR
-- Main Query
SELECT  
"Rule No", Variant,
CASE "Rule No" WHEN 1 THEN 'Certificate Submitted'
               WHEN 2 THEN 'EPA Fail'
			   WHEN 3 THEN 'EPA Pass'
			   WHEN 4 THEN 'Draft Certificate (in UI)'
			   WHEN 5 THEN 'Draft Certificate (in API)'
			   WHEN 6 THEN 'Previous Fail now Draft Certificate (in UI)'
			   WHEN 7 THEN 'Deleted Certificate'
			   WHEN 8 THEN 'No EPA / Certificate'
			   WHEN 9 THEN 'No match'
			   ELSE 'bas data' END "Rule Description",
certificate_by_EPAOrgId "EPAOrgId certificate createdby", ILR_for_EPAOrgId "EPAOrgId on ILR", @epaorgid "Results for EPAO", 
What_is_allowed_for_Certificate "What can EPAO do with Certificate", 
What_is_allowed_for_ILR "What can EPAO do with ILR",
uln, familyname, StandardReference, Larscode, "Version", "Has certificate", "Status=Deleted", OverallGrade, "Status=Draft","Status=Submitted", LatestEpaOutcome, AchievementDate, "Has ILR"
FROM
(
SELECT ROW_NUMBER() OVER (PARTITION BY "Rule No", Larscode ORDER BY ULN) rownumber, 
row_number() OVER (PARTITION BY "Rule No", What_is_allowed_for_Certificate, What_is_allowed_for_ILR, "Has ILR", "Has certificate" order by certificate_createdAt desc) Variant,
*
FROM (
--matches to the GET Certificate rules
SELECT  ab2.uln, CASE WHEN familyname_on_certifiate IS NOT NULL THEN familyname_on_certifiate ELSE familyname_on_ILR END familyname, 
StandardReference, ab2.Larscode, "Has certificate", "Status=Deleted", OverallGrade, "Status=Draft","Status=Submitted", LatestEpaOutcome, AchievementDate, "Version", "Has ILR",
CASE 
-- 1. Certificate Submitted
WHEN "Has certificate" = 'Y' AND "Status=Deleted" = 'N' AND OverallGrade IS NOT NULL AND OverallGrade != 'Fail' AND "Status=Draft" = 'N' AND "Status=Submitted" = 'Y' AND LatestEpaOutcome = 'Pass'                     THEN 1 
-- 2. EPA Fail
WHEN "Has certificate" = 'Y' AND "Status=Deleted" = 'N'                              AND OverallGrade  = 'Fail' AND "Status=Draft" = 'N' AND "Status=Submitted" = 'Y' AND LatestEpaOutcome = 'Fail' AND "Has ILR" = 'Y' THEN 2 
-- 3. EPA Pass
WHEN "Has certificate" = 'Y' AND "Status=Deleted" = 'N' AND OverallGrade IS NULL                                AND "Status=Draft" = 'Y' AND "Status=Submitted" = 'N' AND LatestEpaOutcome = 'Pass' AND "Has ILR" = 'Y' THEN 3 
-- 4. Draft Certificate (in UI)
WHEN "Has certificate" = 'Y' AND "Status=Deleted" = 'N'                                                         AND "Status=Draft" = 'Y' AND "Status=Submitted" = 'N' AND LatestEpaOutcome IS NULL                      THEN 4 
-- 5. Draft Certificate (in API)
WHEN "Has certificate" = 'Y' AND "Status=Deleted" = 'N' AND OverallGrade IS NOT NULL                            AND "Status=Draft" = 'Y' AND "Status=Submitted" = 'N' AND LatestEpaOutcome = 'Pass'                     THEN 5 
-- 6. Previous Fail now back in Draft Certificate in UI
WHEN "Has certificate" = 'Y' AND "Status=Deleted" = 'N'                                                         AND "Status=Draft" = 'Y' AND "Status=Submitted" = 'N' AND LatestEpaOutcome = 'Fail' AND "Has ILR" = 'Y' THEN 6 
-- 7. Deleted Certificate
WHEN "Has certificate" = 'Y' AND "Status=Deleted" = 'Y'                                                         AND "Status=Draft" = 'N' AND "Status=Submitted" = 'N'                               AND "Has ILR" = 'Y' THEN 7 
-- 8. No EPA / Certificate
WHEN "Has certificate" = 'N'                                                                                                                                                                        AND "Has ILR" = 'Y' THEN 8 
-- 9. No match
WHEN "Has certificate" = 'N'                                                                                                                                                                        AND "Has ILR" = 'N' THEN 9 
-- bad data ?
ELSE 9 END "Rule No", certificate_by_EPAOrgId, certificate_CreatedAt, ILR_for_EPAOrgId, @epaorgid Chosen_EPAO, What_is_allowed_for_Certificate, 
CASE WHEN What_is_allowed_for_ILR IS NULL THEN 'No match' ELSE What_is_allowed_for_ILR END What_is_allowed_for_ILR
FROM
(
SELECT 
 uln,  StandardReference, Larscode
,MAX(CASE WHEN recordType = 'certificate' THEN familyname ELSE NULL END) familyname_on_certifiate
,MAX(CASE WHEN recordType = 'ILR' THEN familyname ELSE NULL END) familyname_on_ILR
,MAX(CASE WHEN recordType = 'certificate' THEN 'Y' ELSE 'N' END) "Has certificate"
,MAX(CASE WHEN recordType = 'certificate' THEN (CASE WHEN status = 'Deleted' THEN 'Y' ELSE 'N' END) ELSE NULL END) "Status=Deleted"
,MAX(CASE WHEN recordType = 'certificate' THEN (CASE WHEN status = 'Draft' THEN 'Y' ELSE 'N' END) ELSE NULL END)  "Status=Draft"
,MAX(CASE WHEN recordType = 'certificate' THEN (CASE WHEN status NOT IN ('Deleted','Draft') THEN 'Y' ELSE 'N' END) ELSE NULL END)  "Status=Submitted"
,MAX(CASE WHEN recordType = 'certificate' THEN LatestEpaOutcome ELSE NULL END) LatestEpaOutcome
,MAX(CASE WHEN recordType = 'certificate' THEN OverallGrade ELSE NULL END) OverallGrade
,MAX(CASE WHEN recordType = 'certificate' THEN AchievementDate ELSE NULL END) AchievementDate
,MAX(CASE WHEN recordType = 'certificate' THEN Version ELSE NULL END) "Version"
,MAX(CASE WHEN recordType = 'ILR' THEN 'Y' ELSE 'N' END) "Has ILR"
,MAX(CASE WHEN recordType = 'ILR' THEN status ELSE NULL END) ILR_status
,MAX(CASE WHEN recordType = 'certificate' THEN EpaOrgId ELSE NULL END) certificate_by_EPAOrgId
,MAX(CASE WHEN recordType = 'certificate' THEN CreatedAt ELSE NULL END) certificate_CreatedAt
,MAX(CASE WHEN recordType = 'ILR' THEN EpaOrgId ELSE NULL END) ILR_for_EPAOrgId

FROM 
(
select   'certificate' recordType,ce1.CreatedAt,
  ce1.uln, json_value(ce1.certificatedata, '$.LearnerFamilyName') FamilyName, 
  CASE WHEN ce1.standardUid IS NOT NULL THEN SUBSTRING(ce1.standardUid,1,6) ELSE json_value(ce1.certificatedata, '$.StandardReference') END StandardReference, 
  ce1.standardcode Larscode, 
  ce1.Status,
  json_value(ce1.certificatedata, '$.EpaDetails.LatestEpaOutcome') LatestEpaOutcome, 
  json_value(ce1.certificatedata, '$.OverallGrade') OverallGrade, 
  json_value(ce1.certificatedata, '$.AchievementDate') AchievementDate, 
  json_value(ce1.certificatedata, '$.Version') Version, 
og1.EndPointAssessorOrganisationId EpaOrgId from certificates ce1
join Organisations og1 on og1.id = ce1.OrganisationId
where uln between 1000000001 AND 9999999998
--and uln in ( 6417214655, 1009988667 )
UNION ALL
--
SELECT  'ILR' recordType,il1.CreatedAt,
il1.uln, il1.FamilyName, (select top 1 IFateReferenceNumber from Standards so1 where so1.LarsCode = il1.stdcode) StandardReference, il1.stdcode Larscode,
CASE il1.completionstatus WHEN 1 THEN 'Active' WHEN 2 THEN 'Complete' WHEN 3 THEN  'Withdrawn' WHEN 6 THEN   'Temporarily Withdrawn' ELSE 'Invalid' END status,
NULL LatestEpaOutcome, NULL OverallGrade, NULL AchievementDate, Null Version,  EpaOrgId
from ilrs il1
where uln  between 1000000001 AND 9999999998
--and uln in ( 6417214655, 1009988667 )
) ab1
GROUP BY uln, StandardReference, Larscode
HAVING MAX(StandardReference) = MIN(StandardReference)  -- ignore duff data 
) ab2
LEFT JOIN WhoCanSeeCertificate wh1 on wh1.larscode = ab2.Larscode and wh1.uln = ab2.Uln
LEFT JOIN WhoCanSeeILR wt1 on wt1.larscode = ab2.Larscode and wt1.uln = ab2.Uln
) ab3

) ab4
WHERE 1=1
--AND rownumber = 1
AND Variant = 1                         -- comment this to get multiple examples of the different data scenarios
--AND "Rule No" = 1                     -- uncomment this to focus on a specific rule
--AND "Rule No" NOT IN (0,1)            -- uncomment this to focus on specific rules
ORDER BY "Rule No", certificate_createdAt desc, larscode
