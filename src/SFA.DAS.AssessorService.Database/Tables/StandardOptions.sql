CREATE TABLE [dbo].[StandardOptions]
(
	[StandardUId] VARCHAR(20) NOT NULL,
	[OptionName] NVARCHAR(500) NOT NULL,
)

GO

CREATE INDEX [IX_StandardOptions] ON [dbo].[StandardOptions] ([StandardUId]);
GO

CREATE UNIQUE INDEX [IXU_StandardOptions] ON [dbo].[StandardOptions] ([StandardUId],[OptionName] );
GO

