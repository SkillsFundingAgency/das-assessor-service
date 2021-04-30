/*
	Grant or deny specific database permissions to database roles
*/

GRANT ALTER ON [dbo].[Certificates] TO [database_maintenance] AS [dbo]
GO

GRANT ALTER ON [dbo].[CertificateLogs] TO [database_maintenance] AS [dbo]
GO

GRANT ALTER ON [dbo].[Ilrs] TO [database_maintenance] AS [dbo]
GO

GRANT EXECUTE ON [dbo].[DatabaseMaintenance] TO [database_maintenance] AS [dbo]
GO
