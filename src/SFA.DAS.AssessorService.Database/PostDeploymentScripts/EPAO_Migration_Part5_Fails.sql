-- set CertificateLogs action = 'Submit' , status = 'Submitted' for Fails recorded via API

MERGE INTO CertificateLogs cl1
USING (
SELECT id FROM (
select row_number() over (partition by certificateid order by eventtime desc) rownumber, id from CertificateLogs 
where certificateid in ( Select id from certificates where  createdby = 'API' and status = 'Draft' and  json_value(certificatedata,'$.EpaDetails.LatestEpaOutcome')  = 'Fail'  ) and action = 'Epa'
) ab1 WHERE rownumber = 1 
) upd
ON cl1.id = upd.id 
WHEN MATCHED THEN UPDATE SET  cl1.action = 'Submit' , cl1.status = 'Submitted';


-- set Certificate status = 'Submitted' for Fails recorded via API
MERGE INTO certificates ce1 
USING  (
SELECT id,  json_modify( json_modify(certificatedata,'$.OverallGrade',LatestEpaOutcome),'$.AchievementDate',LatestEpaDate) newcertificatedata
FROM (
select id, certificatedata, json_value(certificatedata,'$.EpaDetails.LatestEpaOutcome') LatestEpaOutcome, json_value(certificatedata,'$.EpaDetails.LatestEpaDate') LatestEpaDate
from certificates where createdby = 'API' and status = 'Draft' and  json_value(certificatedata,'$.EpaDetails.LatestEpaOutcome')  = 'Fail'
) ab1
) upd
ON ce1.id = upd.id
WHEN MATCHED THEN UPDATE SET  ce1.certificatedata =upd.newcertificatedata, ce1.status='Submitted';


