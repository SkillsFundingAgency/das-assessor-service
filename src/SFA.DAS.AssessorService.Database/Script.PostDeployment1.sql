/*
    Post-Deployment Script Template                            
    --------------------------------------------------------------------------------------
    This file contains SQL statements that will be appended to the build script.        
    Use SQLCMD syntax to include a file in the post-deployment script.            
    Example:      :r .\myfile.sql                                
    Use SQLCMD syntax to reference a variable in the post-deployment script.        
    Example:      :setvar TableName MyTable                            
                SELECT * FROM [$(TableName)]                    
    --------------------------------------------------------------------------------------

    This is typically used to manipulate the data after running the DAC PAC scripts, where this
    is required to allow constraints.

    Each script should be stored under the PostDeploymentScripts folder and will be run on each deployment,
    so all scripts should be written as IDEMPOTENT.
  
    When a script has been deployed to PROD it can be disabled by removing the reference below and optionally retained
    under the PostDeploymentScripts folder for future reference.
*/

/*	NORMALLY KEEP SECTION FIRST - START DATABASE LOOKUP */
:r .\PostDeploymentScripts\LookupData\SynchronizeLookupData.sql
/* 	NORMALLY KEEP SECTION FIRST - END DATABASE LOOKUP */

-- ON-613 Patch Certificates with STxxxx StandardReference, where it is not yet included. 
-- AB 11/03/19 Keep this active for new deployments, for now
-- AB 31/07/19 Still seeing existance of certs without Standard reference (need to understand why)
-- ****************************************************************************
MERGE INTO certificates ma1
USING (
SELECT ce1.[Id],JSON_MODIFY([CertificateData],'$.StandardReference',st1.ReferenceNumber) newData
  FROM [Certificates] ce1 
  JOIN [StandardCollation] st1 ON ce1.StandardCode = st1.StandardId
  WHERE st1.ReferenceNumber IS NOT NULL 
  AND JSON_VALUE([CertificateData],'$.StandardReference') IS NULL) up1
ON (ma1.id = up1.id)
WHEN MATCHED THEN UPDATE SET ma1.[CertificateData] = up1.[newData];
