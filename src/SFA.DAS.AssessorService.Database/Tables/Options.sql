CREATE TABLE [dbo].[Options]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [StdCode] INT NOT NULL, 
    [OptionName] NVARCHAR(MAX) NOT NULL
)

GO

CREATE INDEX [IX_Options_LarsCode] ON [dbo].[Options] ([StdCode])
