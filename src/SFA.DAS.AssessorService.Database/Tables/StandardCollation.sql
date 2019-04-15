CREATE TABLE [dbo].[StandardCollation]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY, 
    [StandardId] INT NOT NULL, 
    [ReferenceNumber] NVARCHAR(10) NULL,
	[Title] NVARCHAR(500)  NOT NULL,
	[StandardData] NVARCHAR(MAX) NULL,
	[DateAdded] DateTime NOT NULL DEFAULT GETUTCDATE(),
	[DateUpdated] DATETIME NULL,
	[DateRemoved] DATETIME NULL,
	[IsLive] BIT DEFAULT 1
)


CREATE NONCLUSTERED INDEX [IX_StandardCollation_Title_StandardData] ON [StandardCollation] ([StandardId]) INCLUDE ([Title],[StandardData])
GO