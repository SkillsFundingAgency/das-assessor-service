CREATE TABLE [dbo].[ContactRoles](
	[Id] [uniqueidentifier] NOT NULL,
	[ContactId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NULL,
	[DeletedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL, 
    CONSTRAINT [PK_ContactRoles] PRIMARY KEY ([Id])
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ContactRoles]  WITH CHECK ADD  CONSTRAINT [FK_ContactRoles_Contacts_ContactId] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([Id])
GO

ALTER TABLE [dbo].[ContactRoles] CHECK CONSTRAINT [FK_ContactRoles_Contacts_ContactId]
GO

ALTER TABLE [dbo].[ContactRoles]  WITH CHECK ADD  CONSTRAINT [FK_ContactRoles_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
GO

ALTER TABLE [dbo].[ContactRoles] CHECK CONSTRAINT [FK_ContactRoles_Roles_RoleId]
GO
