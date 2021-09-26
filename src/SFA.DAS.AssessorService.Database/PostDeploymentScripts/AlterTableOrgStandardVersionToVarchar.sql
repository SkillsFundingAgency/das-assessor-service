/*
Post-Deployment Script 							
-----------------------------------------------------------------------------------------------------
Legacy decimal(18,1) datatype for version field on OrganisationStandardVersion needs to be varchar(20)
-----------------------------------------------------------------------------------------------------
*/

IF (EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = 'dbo' 
            AND  TABLE_NAME = 'OrganisationStandardVersion'))
BEGIN
  ALTER TABLE  [dbo].[OrganisationStandardVersion]
  ALTER COLUMN [Version] VARCHAR(20)
END




