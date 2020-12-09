CREATE TABLE [dbo].[Privileges](
    [Id] [uniqueidentifier] NOT NULL,
    [UserPrivilege] [nvarchar](120) NOT NULL,
    [MustBeAtLeastOneUserAssigned] [bit] NOT NULL,
    [Description] [nvarchar](MAX) NOT NULL,
    [PrivilegeData] [nvarchar](MAX) NOT NULL,
    [Key] [nvarchar](125) NOT NULL,
    [Enabled] bit NOT NULL
 CONSTRAINT [PK_Privileges] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Key] ON [dbo].[Privileges]
(
	[Key] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO
