-- This script was extracted from the PreDeploymentScript it has currently worked its way 
-- into PROD and so has been removed from the PreDeploymentScript

IF EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'CertificateLogs'
    AND COLUMN_NAME = 'WasRejected')
BEGIN
  ALTER TABLE [dbo].[CertificateLogs]
    DROP COLUMN [WasRejected]
END;
GO