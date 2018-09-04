/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Certificates]') 
         AND name = 'CreateDay'
)
BEGIN
	SET NOEXEC ON;
END
ALTER TABLE [Certificates] 
ADD [CreateDay] date NULL;
GO
-- Step 2
UPDATE [Certificates] 
SET [CreateDay] = CONVERT(date, CreatedAt)
WHERE CertificateReferenceId >= 10000;

MERGE INTO [Certificates] ce1
USING (
select cl.CertificateId, cl.EventTime from CertificateLogs cl
where cl.CertificateId in
(
	SELECT id
	FROM [dbo].Certificates
	WHERE CertificateReferenceId < 10000
	)
and cl.Action = 'submit' ) ab1
ON (ce1.id = ab1.Certificateid)
WHEN MATCHED THEN UPDATE 
SET [CreateDay] = CONVERT(date, ab1.EventTime);

-- Step 3
UPDATE Certificates 
SET Uln = 0-CertificateReferenceId
where Uln = 0;

-- Step 4
ALTER TABLE [Certificates] 
ALTER COLUMN [CreateDay] date NOT NULL;
GO

CREATE UNIQUE INDEX [IXU_Certificates] ON [Certificates] ([Uln], [StandardCode], [CreateDay]);
	
SET NOEXEC OFF;