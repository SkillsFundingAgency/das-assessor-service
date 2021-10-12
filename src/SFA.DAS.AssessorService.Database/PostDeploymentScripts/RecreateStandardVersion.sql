/*
	Recreate Standard Version from StandardUId
*/

IF (EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = 'dbo' 
            AND  TABLE_NAME = 'OrganisationStandardVersion'))
BEGIN
    UPDATE [dbo].[OrganisationStandardVersion] SET [Version] = RIGHT([StandardUId],3)
END