CREATE TABLE [dbo].[Audits](
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [OrganisationId] UNIQUEIDENTIFIER NOT NULL, 
    [UpdatedBy] NVARCHAR(30) NOT NULL, 
    [UpdatedAt] DATETIME2 NOT NULL, 
    [AuditData] NVARCHAR(MAX) NOT NULL)
GO

ALTER TABLE [dbo].[Audits] ADD CONSTRAINT [FK_Organisations_Audits] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisations] ([Id]);
GO
ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_Organisations_Audits];
GO