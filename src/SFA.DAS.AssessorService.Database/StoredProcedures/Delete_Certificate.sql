-- Soft Delete a certificates Record
CREATE PROCEDURE [Delete_Certificate] 
@CertificateID Int
AS


UPDATE certificates 
SET [status] = 'Deleted', [DeletedBy] = 'manual', [DeletedAt] = GETDATE() , [IsPrivatelyFunded] = 0, [PrivatelyFundedStatus] = NULL
WHERE [certificatereferenceid] = @CertificateID;

UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactName',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactOrganisation',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactAddLine1',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactAddLine2',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactAddLine3',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactAddLine4',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactPostCode',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.AchievementDate',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.CourseOption',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.OverallGrade',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.Department',null) WHERE [certificatereferenceid] = @CertificateID;
UPDATE certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.EpaDetails',null) WHERE [certificatereferenceid] = @CertificateID;


-- log diffs since last change - these will have been manually amended
INSERT INTO [CertificateLogs] ([ID],[Action],[CertificateId],[EventTime],[Status],[CertificateData],[Username],[BatchNumber],[ReasonforChange])
SELECT NEWID() Id, 'Deleted' [Action],ce.[id] [CertificateId], 
GETDATE() [EventTime], ce.[Status], ce.[CertificateData], 'manual' [Username], ce.[BatchNumber], 'Deleted'
 FROM [Certificates] ce 
 LEFT JOIN [CertificateLogs] lgs 
 ON lgs.CertificateId = ce.Id AND lgs.Action = 'Deleted'  AND lgs.EventTime > DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)
 WHERE [certificatereferenceid] = @CertificateID
  AND lgs.Id IS NULL;


