-- Migrate part 4 Apply table - set the Standardreference

UPDATE Apply
SET StandardReference = JSON_VALUE(ApplyData,'$.Apply.StandardReference') 
WHERE StandardReference IS NULL AND StandardCode IS NOT NULL


-- SV-879 Patch Apply
-- patch Version in to existing applications, for standard and standardwithdrawal
MERGE INTO APPLY masterApply
USING (
SELECT * FROM (
SELECT ApplyId, Versions, REPLACE(REPLACE(ApplyData,'"Apply": {','"Apply":{'),'"Apply":{','"Apply":{'+Versions+',') NewApplyData
FROM (
SELECT ApplyId, Applydata,'"Versions":['+string_agg('"'+cast(version as varchar(max))+'"',', ') WITHIN GROUP (ORDER BY IFateReferenceNumber ASC) +']' Versions, MAX(standardUid) standardUid
FROM (
SELECT Id ApplyId, version, standardUid, IFateReferenceNumber,Applydata,so1.versionearlieststartdate, dated
FROM (
SELECT Id, Applydata, StandardReference,
CASE 
WHEN ApplicationStatus = 'Approved' AND ApprovedDate is not null THEN convert(date,ApprovedDate) 
WHEN (SequenceNo = 2 AND JSON_VALUE(ApplyData,'$.Apply.StandardSubmissionClosedDate') is not null ) THEN convert(date,JSON_VALUE(ApplyData,'$.Apply.StandardSubmissionClosedDate'))
WHEN (SequenceNo = 4 AND JSON_VALUE(ApplyData,'$.Apply.StandardWithdrawalSubmissionClosedDate') is not null ) THEN convert(date,JSON_VALUE(ApplyData,'$.Apply.StandardWithdrawalSubmissionClosedDate'))
ELSE convert(date,createdat) END dated
FROM Apply 
CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo Int, NotRequired BIT, IsActive BIT, ApprovedDate char(200) ) 
WHERE DeletedAt IS NULL
AND StandardCode IS NOT NULL 
AND SequenceNo In ( 2 , 4)
AND NotRequired = 0
AND JSON_QUERY(ApplyData,'$.Apply.Versions') IS NULL
) ap1
JOIN Standards so1 ON so1.IFateReferenceNumber = ap1.StandardReference 
WHERE dated >= so1.versionearlieststartdate
) ab1
GROUP BY ApplyId,IFateReferenceNumber,Applydata
) ab2 
) ab3 WHERE ISJSON( NewApplyData) > 0 AND JSON_QUERY(NewApplyData,'$.Apply.Versions') IS NOT NULL
) upd
ON masterApply.id = upd.ApplyId
WHEN MATCHED THEN UPDATE SET masterApply.ApplyData = upd.NewApplyData;

