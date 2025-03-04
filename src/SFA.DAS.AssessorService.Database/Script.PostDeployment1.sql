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

    Each temporary script should be stored under the 'PostDeploymentScripts\TemporaryPatches' folder prefixed with the Jira ticket number
    for which it has been created and will be run on each deployment, so all scripts should be written as IDEMPOTENT.
  
    When a script has been deployed to ALL environments it can be removed.
*/

:r .\PostDeploymentScripts\TemporaryPatches\ON-613_Patch_standard_reference.sql
:r .\PostDeploymentScripts\TemporaryPatches\P2-2241_Drop_apar_tables.sql
:r .\PostDeploymentScripts\TemporaryPatches\P2-2340_Create_framework_tables.sql