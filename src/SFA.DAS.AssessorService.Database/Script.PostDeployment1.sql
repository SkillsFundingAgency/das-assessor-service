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

-- backup ILRS before data synch
/* DONE
DELETE FROM IlrsCopy

INSERT INTO IlrsCopy SELECT * FROM Ilrs
*/

--- STORY ON-1392 ordering delivery area as per UX requirements
/* DONE
update deliveryarea set Ordering=1 where Area='North East'
update deliveryarea set Ordering=2 where Area='North West'
update deliveryarea set Ordering=3 where Area='Yorkshire and the Humber'
update deliveryarea set Ordering=4 where Area='East Midlands'
update deliveryarea set Ordering=5 where Area='West Midlands'
update deliveryarea set Ordering=6 where Area='East of England'
update deliveryarea set Ordering=7 where Area='London'
update deliveryarea set Ordering=8 where Area='South East'
update deliveryarea set Ordering=9 where Area='South West*/

alter table Contacts alter column Status nvarchar(20) not null
alter table EMailTemplates alter column Recipients nvarchar(max) NULL

IF NOT EXISTS (SELECT * FROM EMailTemplates WHERE TemplateName = N'EPAOUserApproveConfirm')
BEGIN
INSERT EMailTemplates VALUES (N'4df42e62-c08f-4e1c-ae8e-7ddf599ed3f6', N'EPAOUserApproveConfirm', N'539204f8-e99a-4efa-9d1f-d0e58b26dd7b', NULL, GETDATE(), NULL, NULL)
END
alter table EMailTemplates alter column Recipients nvarchar(max) null

/* DONE
-- update FHA details STORY ON-1058
:r UpdateFHADetails.sql
*/

/* DONE
-- load December 2018 report DATABASE
:r setDec18EPAReport.sql
*/
