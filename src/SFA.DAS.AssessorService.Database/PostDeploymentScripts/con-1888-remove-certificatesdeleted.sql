/*
	This script will drop the CertificatesDeleted and CertificateLogsDeleted table which were used to hold certificates
	which were deleted due to be detected as duplicates, these only existing for a short time during beta test and
	the detection has now been removed from PostDeploy.
*/

BEGIN TRANSACTION

IF OBJECT_ID('dbo.CertificatesDeleted', 'U') IS NOT NULL 
  DROP TABLE [dbo].[CertificatesDeleted]; 

IF OBJECT_ID('dbo.CertificateLogsDeleted', 'U') IS NOT NULL 
  DROP TABLE [dbo].[CertificateLogsDeleted]; 

COMMIT TRANSACTION