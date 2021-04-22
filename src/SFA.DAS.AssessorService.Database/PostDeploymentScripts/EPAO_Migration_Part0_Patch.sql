/*
patch Certificates.CertificateData "version" to "Version"					
*/

UPDATE  Certificates
SET CertificateData =
CASE WHEN  JSON_VALUE(CertificateData,'$.Version')  IS NULL
THEN JSON_MODIFY(JSON_MODIFY(CertificateData,'$.version', null),'$.Version','')
ELSE JSON_MODIFY(CertificateData,'$.version', null)
END
where JSON_VALUE(CertificateData,'$.version')  = '' 
and StandardUId IS NOT NULL

