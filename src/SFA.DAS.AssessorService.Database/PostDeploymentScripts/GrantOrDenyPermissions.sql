/*
	Grant or deny specific database permissions to database roles
*/

GRANT ALTER ON [dbo].[Certificates] TO [database_maintenance] AS [dbo]
GO

GRANT EXECUTE ON [dbo].[DatabaseMaintenace] TO [database_maintenance] AS [dbo]
GO
