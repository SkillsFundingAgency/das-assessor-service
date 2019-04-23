CREATE TABLE [dbo].[ContactRoles](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](255) NULL,
	[ContactId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ContactRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]