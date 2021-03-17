-- Migration part 3 - OrganisationStandardVersions population
-- All standardversions for each organisation, that were active during the EffectiveFrom date and EffectiveTo date (if set) or ongoing
MERGE INTO OrganisationStandardVersion MasterOSV
USING (
SELECT  
       st1.[StandardUId] 
      ,st1.[Version]
      ,os1.id [OrganisationStandardId]
	  ,CASE WHEN os1.[EffectiveFrom] > st1.VersionEarliestStartdate THEN os1.[EffectiveFrom] ELSE st1.VersionEarliestStartdate END EffectiveFrom
      ,os1.[EffectiveTo]
	  ,os1.DateStandardApprovedOnRegister  [DateVersionApproved]
      ,'Applied versions retrospectively, based on version '+TRIM(CONVERT(CHAR,st1.version))+' Earliest start date '+CONVERT(CHAR(10),VersionEarliestStartdate,121) [Comments]
      ,'Live' [Status]
FROM OrganisationStandard os1
JOIN Standards st1 ON st1.ifatereferencenumber = os1.StandardReference
WHERE os1.Status = 'Live'
AND  (os1.[EffectiveTo] IS NULL OR  os1.[EffectiveTo] >=  st1.VersionEarliestStartdate ) 
) upd
ON (MasterOSV.StandardUId = upd.StandardUId AND masterOSV.[OrganisationStandardId] = upd.[OrganisationStandardId])
WHEN NOT MATCHED THEN
INSERT VALUES (upd.[StandardUId], upd.[Version], upd.[OrganisationStandardId], upd.[EffectiveFrom], upd.[EffectiveTo], upd.[DateVersionApproved], upd.[Comments], upd.[Status]);

