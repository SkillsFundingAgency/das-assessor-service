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

--- STORY ON-1392 ordering delivery area as per UX requirements
update deliveryarea set Ordering=1 where Area='North East'
update deliveryarea set Ordering=2 where Area='North West'
update deliveryarea set Ordering=3 where Area='Yorkshire and the Humber'
update deliveryarea set Ordering=4 where Area='East Midlands'
update deliveryarea set Ordering=5 where Area='West Midlands'
update deliveryarea set Ordering=6 where Area='East of England'
update deliveryarea set Ordering=7 where Area='London'
update deliveryarea set Ordering=8 where Area='South East'
update deliveryarea set Ordering=9 where Area='South West'


-- update FHA details STORY ON-1058
:r UpdateFHADetails.sql