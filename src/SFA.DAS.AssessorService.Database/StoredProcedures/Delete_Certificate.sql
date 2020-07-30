-- Soft Delete a certificates Record
-- this is for manual use only, and not for programatic use
-- as will need to know the user that is deleting the certificate
CREATE PROCEDURE [Delete_Certificate] 
@CertificateID Int
AS


UPDATE Certificates 
SET [Status] = 'Deleted', [DeletedBy] = 'manual', [DeletedAt] = GETDATE() , [IsPrivatelyFunded] = 0, [PrivatelyFundedStatus] = NULL
WHERE [CertificateReferenceId] = @CertificateID;

UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactName',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactOrganisation',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactAddLine1',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactAddLine2',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactAddLine3',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactAddLine4',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.ContactPostCode',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.AchievementDate',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.CourseOption',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.OverallGrade',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.Department',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.EpaDetails',null) WHERE [CertificateReferenceId] = @CertificateID;
-- learner details
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.LearnerGivenNames',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.LearnerFamilyName',null) WHERE [CertificateReferenceId] = @CertificateID;
UPDATE Certificates SET [CertificateData] = JSON_MODIFY([CertificateData],'strict $.FullName',null) WHERE [CertificateReferenceId] = @CertificateID;


-- log diffs since last change - these will have been manually amended
INSERT INTO [CertificateLogs] ([Id],[Action],[CertificateId],[EventTime],[Status],[CertificateData],[Username],[BatchNumber],[ReasonForChange])
SELECT NEWID() Id, 'Deleted' [Action],ce.[Id] [CertificateId], 
GETDATE() [EventTime], ce.[Status], ce.[CertificateData], 'manual' [Username], ce.[BatchNumber], 'Deleted'
 FROM [Certificates] ce 
 LEFT JOIN [CertificateLogs] lgs 
 ON lgs.CertificateId = ce.Id AND lgs.Action = 'Deleted'  AND lgs.EventTime > DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)
 WHERE [CertificateReferenceId] = @CertificateID
  AND lgs.Id IS NULL;


