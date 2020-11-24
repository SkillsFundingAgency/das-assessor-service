
CREATE TABLE [dbo].[ContactsPrivileges](
	[ContactId] [uniqueidentifier] NOT NULL,
	[PrivilegeId] [uniqueidentifier] NOT NULL
UNIQUE NONCLUSTERED 
(
	[ContactId] ASC,
	[PrivilegeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ContactsPrivileges] ADD CONSTRAINT [FK_ContactsPrivileges_Privileges] FOREIGN KEY([PrivilegeId])
REFERENCES [dbo].[Privileges] ([Id])
GO
