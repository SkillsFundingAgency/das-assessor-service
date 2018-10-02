CREATE TABLE [dbo].[StaffReports]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[ReportName] [nvarchar](max) NOT NULL,
	[StoredProcedure] [nvarchar](max) NOT NULL,
	[CreatedAt]    DATETIME2 (7)    NOT NULL DEFAULT GETDATE(),
    [DeletedAt]    DATETIME2 (7)    NULL,
    [UpdatedAt]    DATETIME2 (7)    NULL,
)

GO
