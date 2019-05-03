/* Add pre Deployment SQL here */

IF EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'CertificateLogs'
    AND COLUMN_NAME = 'WasRejected')
BEGIN
  ALTER TABLE CertificateLogs
    DROP COLUMN WasRejected
END;
GO
