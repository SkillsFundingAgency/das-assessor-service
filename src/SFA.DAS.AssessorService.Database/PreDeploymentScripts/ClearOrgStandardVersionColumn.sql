/*
	Clear standard version from OrganisationStandardVersion
*/

IF (EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = 'dbo' 
            AND  TABLE_NAME = 'OrganisationStandardVersion'))
BEGIN
    UPDATE [dbo].[OrganisationStandardVersion] set Version = null
END