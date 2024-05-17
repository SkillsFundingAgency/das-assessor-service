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
*/

/* START OF PERMANENT SCRIPT SECTION */ 
:r .\PostDeploymentScripts\LookupData\SynchronizeLookupData.sql
:r .\PostDeploymentScripts\GrantOrDenyPermissions.sql
/* END OF PERMANENT SCRIPT SECTION */ 

/*  START OF TEMPORARY SCRIPT SECTION

    Each temporary script should be stored under the 'PostDeploymentScripts' folder prefixed with the Jira ticket number
    for which it has been created and will be run on each deployment, so all scripts should be written as IDEMPOTENT.
  
    When a script has been deployed to ALL environments it can be disabled by removing the reference below and optionally retained
    under the PostDeploymentScripts folder for future reference.
*/

-- CON-3359
:r .\PostDeploymentScripts\CON-3359_Remove_IlrsImport.sql

-- ON-613 Patch Certificates with STxxxx StandardReference, where it is not yet included. 
-- AB 11/03/19 Keep this active for new deployments, for now
-- AB 31/07/19 Still seeing existance of certs without Standard reference (need to understand why)
-- ****************************************************************************
-- AB 10/05/21 Keeping this for now to patch FAILs recorded via the API
;
WITH Standards_CTE AS(
SELECT ROW_NUMBER() OVER (PARTITION BY Ifatereferencenumber ORDER BY VersionMajor DESC, VersionMinor DESC) seq, * FROM Standards WHERE LarsCode != 0)

MERGE INTO certificates ma1
USING (
SELECT ce1.[Id],JSON_MODIFY([CertificateData],'$.StandardReference', st1.IFateReferenceNumber) newData
  FROM [Certificates] ce1 
  JOIN Standards_CTE st1 ON ce1.StandardCode = st1.LarsCode and st1.seq = 1
  WHERE st1.IFateReferenceNumber IS NOT NULL 
  AND JSON_VALUE([CertificateData],'$.StandardReference') IS NULL) up1
ON (ma1.id = up1.id)
WHEN MATCHED THEN UPDATE SET ma1.[CertificateData] = up1.[newData];

--SV-1290 Remove Un-necessary tbles
:r .\PostDeploymentScripts\SV-1290-RemoveOldStandardCollationTables.sql

--QF-1366 Map OFQUAL RN to EPAOrgId
:r .\PostDeploymentScripts\QF-1366-MapOFQUALRNtoEPAOrgId.sql

--QF-1912 Add Index to Contact.GovUkIdentifier
:r .\PostDeploymentScripts\QF-1912-AddIndexToContacts-GovUkIdentifier.sql

--QF-1912 Update Pending users to Live
:r .\PostDeploymentScripts\QF-1912-UpdatePendingUsersToLive.sql
