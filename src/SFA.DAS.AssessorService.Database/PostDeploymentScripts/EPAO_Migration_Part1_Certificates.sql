-- migration for EPAO service
WITH standardversions 
AS (
SELECT standardUid, IfateReferenceNumber StandardReference,  version, earlieststartdate, LARScode
-- 
,CASE WHEN PreviousStatus1 != '0' THEN PreviousVersion1 END PreviousVersion1
,CASE WHEN PreviousStatus1 != '0' THEN convert(char(10),Previouslateststartdate1,121) END Previouslateststartdate1
--
,CASE WHEN PreviousStatus2 != '0' THEN PreviousVersion2 END PreviousVersion2
,CASE WHEN PreviousStatus2 != '0' THEN convert(char(10),Previouslateststartdate2,121) END Previouslateststartdate2
--
,CASE WHEN PreviousStatus3 != '0' THEN PreviousVersion3 END PreviousVersion3
,CASE WHEN PreviousStatus3 != '0' THEN convert(char(10),Previouslateststartdate3,121) END Previouslateststartdate3
--
,CASE WHEN PreviousStatus4 != '0' THEN PreviousVersion4 END PreviousVersion4
,CASE WHEN PreviousStatus4 != '0' THEN convert(char(10),Previouslateststartdate4,121) END Previouslateststartdate4
--
FROM(
SELECT co1.standardUid, co1.IfateReferenceNumber,  version, VersionEarliestStartDate earlieststartdate, LARScode
,ROW_NUMBER() OVER (PARTITION BY co1.IfateReferenceNumber order by version desc) as rownumber
--
, LAG(status, 1,0) OVER (partition by  co1.IfateReferenceNumber order by version ) AS PreviousStatus1 
, LAG(version, 1,0) OVER (partition by  co1.IfateReferenceNumber order by version ) AS PreviousVersion1 
, LAG(VersionLatestStartDate, 1,0) OVER (partition by  co1.IfateReferenceNumber order by version ) AS Previouslateststartdate1 
--
 , LAG(status, 2,0) OVER (partition by  co1.IfateReferenceNumber order by version ) AS PreviousStatus2 
 ,LAG(version, 2,0) OVER (partition by co1.IfateReferenceNumber order by version ) AS PreviousVersion2
, LAG(VersionLatestStartDate, 2,0) OVER (partition by  co1.IfateReferenceNumber order by version ) AS Previouslateststartdate2
--
 , LAG(status, 3,0) OVER (partition by  co1.IfateReferenceNumber order by version ) AS PreviousStatus3 
 ,LAG(version, 3,0) OVER (partition by co1.IfateReferenceNumber order by version ) AS PreviousVersion3
, LAG(VersionLatestStartDate, 3,0) OVER (partition by  co1.IfateReferenceNumber order by version ) AS Previouslateststartdate3
--
 , LAG(status, 4,0) OVER (partition by  co1.IfateReferenceNumber order by version ) AS PreviousStatus4 
 ,LAG(version, 4,0) OVER (partition by co1.IfateReferenceNumber order by version ) AS PreviousVersion4
, LAG(VersionLatestStartDate, 4,0) OVER (partition by  co1.IfateReferenceNumber order by version ) AS Previouslateststartdate4
FROM
[Standards] co1  
) ab2
WHERE rownumber = 1 AND EarliestStartDate IS NOT NULL
),
certs AS
-- certificates
(SELECT certificateid,standardcode,standardreference,CONVERT(datetime,LearningStartDate) LearnStartDate
FROM 
(
SELECT [Id] certificateid, standardcode
,JSON_VALUE([CertificateData],'$.StandardReference') StandardReference
,JSON_VALUE([CertificateData],'$.StandardLevel') StandardLevel
,CONVERT(CHAR(10),JSON_VALUE([CertificateData],'$.LearningStartDate')) LearningStartDate
  FROM [dbo].[Certificates] 
  WHERE JSON_VALUE([CertificateData],'$.LearningStartDate') IS NOT NULL
  ) ce1
) 
MERGE INTO certificates MasterCerts
USING(
SELECT StandardReference+'_'+TRIM(Convert(char,EstimatedVersion)) EstimatedUid, *
FROM(
SELECT CASE 
WHEN learnStartdate <= Previouslateststartdate4 THEN previousversion4
WHEN learnStartdate <= Previouslateststartdate3 THEN previousversion3
WHEN learnStartdate <= Previouslateststartdate2 THEN previousversion2
WHEN learnStartdate <= Previouslateststartdate1 THEN previousversion1 
ELSE version
END EstimatedVersion,certs.* FROM certs
JOIN  standardversions s1 on s1.StandardReference = certs.StandardReference 
) ab3 ) ceupd
ON MasterCerts.id = ceupd.certificateid 
WHEN matched AND MasterCerts.standardUId IS NULL 
THEN  UPDATE 
SET MasterCerts.standardUId = ceupd.EstimatedUId,
      MasterCerts.Certificatedata = JSON_MODIFY(MasterCerts.Certificatedata,'$.Version',''); 
-- set version as an empty string (as it is not known!)


