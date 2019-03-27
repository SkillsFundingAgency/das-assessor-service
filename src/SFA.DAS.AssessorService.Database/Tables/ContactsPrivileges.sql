
CREATE TABLE [dbo].[ContactsPrivileges](
	[ContactId] [uniqueidentifier] NOT NULL,
	[PrivilegeId] [uniqueidentifier] NOT NULL,
UNIQUE NONCLUSTERED 
(
	[ContactId] ASC,
	[PrivilegeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/*
ALTER TABLE [dbo].[ContactsPrivileges]  WITH CHECK ADD FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([Id])
GO

ALTER TABLE [dbo].[ContactsPrivileges]  WITH CHECK ADD  CONSTRAINT [FK__ContactsR__PrivilegeI__09746778] FOREIGN KEY([PrivilegeId])
REFERENCES [dbo].[Privileges] ([Id])
GO

ALTER TABLE [dbo].[ContactsPrivileges] CHECK CONSTRAINT [FK__ContactsR__PrivilegeI__09746778]
GO
*/