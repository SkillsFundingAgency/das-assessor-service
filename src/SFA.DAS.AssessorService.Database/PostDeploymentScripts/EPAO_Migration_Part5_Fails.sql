-- Migration Part5

-- set CertificateLogs action = 'Submit' , status = 'Submitted' for Fails recorded via API

UPDATE CertificateLogs 
SET action = 'Submit' , status = 'Submitted'
WHERE id IN ( SELECT id FROM (
select row_number() over (partition by certificateid order by eventtime desc) rownumber, id from CertificateLogs 
where certificateid in ( Select id from certificates where  createdby = 'API' and status = 'Draft' and  json_value(certificatedata,'$.EpaDetails.LatestEpaOutcome')  = 'Fail'  ) and action = 'Epa'
) ab1 WHERE rownumber = 1  )
