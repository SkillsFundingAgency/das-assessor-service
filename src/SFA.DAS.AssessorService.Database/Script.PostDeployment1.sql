﻿/*
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

-- backup ILRS before data synch
/* DONE
DELETE FROM IlrsCopy

INSERT INTO IlrsCopy SELECT * FROM Ilrs
*/

/* DONE
update deliveryarea set Ordering=1 where Area='North East'
update deliveryarea set Ordering=2 where Area='North West'
update deliveryarea set Ordering=3 where Area='Yorkshire and the Humber'
update deliveryarea set Ordering=4 where Area='East Midlands'
update deliveryarea set Ordering=5 where Area='West Midlands'
update deliveryarea set Ordering=6 where Area='East of England'
update deliveryarea set Ordering=7 where Area='London'
update deliveryarea set Ordering=8 where Area='South East'
update deliveryarea set Ordering=9 where Area='South West'
*/

-- ON-1374 update any new organisation standards to 'Live' if minimum acceptance criteria for live is available
UPDATE organisationStandard 
	SET Status='Live', 
	DateStandardApprovedOnRegister = ISNULL(DateStandardApprovedOnRegister, CONVERT(DATE, GETDATE()))
	WHERE Id IN (SELECT organisationStandardId FROM  OrganisationStandardDeliveryArea)
	AND contactId IS NOT NULL
	AND Status='New'

/* DONE
-- ON-1058 update FHA details STORY 
:r UpdateFHADetails.sql
*/

/* DONE
-- load December 2018 report DATABASE
:r setDec18EPAReport.sql
*/

-- patch FundingModel, where this was not set by data sync
UPDATE Ilrs SET FundingModel = 36 WHERE FundingModel IS NULL

/* DONE
-- fix options
UPDATE [Certificates]
SET [CertificateData] = JSON_MODIFY([CertificateData], '$.CourseOption','Alcoholic Beverage Service') 
WHERE json_value(certificatedata,'$.CourseOption') = 'Alcholic beverage service'

UPDATE [Options] 
SET [OptionName] = 'Alcoholic Beverage Service'
WHERE [OptionName] = 'Alcholic beverage service'
*/

-- ON-613 Patch Certificates with STxxxx StandardReference, where it is not yet included. 
-- AB 11/03/19 Keep this active for new deployments, for now
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

-- ****************************************************************************

