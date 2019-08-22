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


-- START OF ON-1952
UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'HEI' AND FinancialExempt = 0
UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'Public Sector' AND FinancialExempt = 0
UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'College' AND FinancialExempt = 0 
UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'Academy or Free School' AND FinancialExempt = 0
-- END OF ON-1952

-- ON-2197 - PostCode to Region Mapping 
:r .\Insert-Postcode-to-Regions.sql

-- ON-2222 - remove duplicated certs
:r .\Delete-Duplicated-Certs.sql
