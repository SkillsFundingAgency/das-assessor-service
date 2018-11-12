CREATE TABLE [dbo].[StandardCollation]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [StandardId] INT NOT NULL, 
    [ReferenceNumber] NVARCHAR(10) NULL,
	[Title] NVARCHAR(500)  NOT NULL,
	[StandardData] NVARCHAR(MAX) NULL,
	[DateAdded] DateTime NOT NULL DEFAULT GETUTCDATE(),
	[DateUpdated] DATETIME NULL,
	[DateRemoved] DATETIME NULL,
	[IsLive] BIT DEFAULT 1
)
