-- ON-613 Patch Certificates with STxxxx StandardReference, where it is not yet included. 
-- AB 11/03/19 Keep this active for new deployments, for now
-- AB 31/07/19 Still seeing existance of certs without Standard reference (need to understand why)
-- ****************************************************************************
-- AB 10/05/21 Keeping this for now to patch FAILs recorded via the API
;
WITH Standards_CTE AS(
SELECT ROW_NUMBER() OVER (PARTITION BY Ifatereferencenumber ORDER BY VersionMajor DESC, VersionMinor DESC) seq, * FROM Standards WHERE LarsCode != 0)

MERGE INTO Certificates ma1
USING (
SELECT ce1.[Id],JSON_MODIFY([CertificateData],'$.StandardReference', st1.IFateReferenceNumber) newData
  FROM [StandardCertificates] ce1 
  JOIN Standards_CTE st1 ON ce1.StandardCode = st1.LarsCode and st1.seq = 1
  WHERE st1.IFateReferenceNumber IS NOT NULL 
  AND JSON_VALUE([CertificateData],'$.StandardReference') IS NULL) up1
ON (ma1.id = up1.id)
WHEN MATCHED THEN UPDATE SET ma1.[CertificateData] = up1.[newData];