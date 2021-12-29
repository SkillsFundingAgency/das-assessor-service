/*
    Pre-Deployment Script Template                            
    --------------------------------------------------------------------------------------
    This file contains SQL statements that will be prepended to the build script.        
    Use SQLCMD syntax to include a file in the pre-deployment script.            
    Example:      :r .\myfile.sql                                
    Use SQLCMD syntax to reference a variable in the pre-deployment script.        
    Example:      :setvar TableName MyTable                            
                SELECT * FROM [$(TableName)]                    
    --------------------------------------------------------------------------------------

    This is typically used to manipulate the schema and data prior to running the DAC PAC scripts, where this
    is required to allow constraints to be added and checked or where the removal of schema would cause the
    DAC PAC script to fail due to it being currently set to disallow data dropping as a safety feature.
    
    Each script should be stored under the PreDeploymentScripts folder and will be run on each deployment,
    so all scripts should be written as IDEMPOTENT.
      
    When a script has been deployed to PROD it can be disabled by removing the reference below and optionally retained
    under the PreDeploymentScripts folder for future reference.
*/