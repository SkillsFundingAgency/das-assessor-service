
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

