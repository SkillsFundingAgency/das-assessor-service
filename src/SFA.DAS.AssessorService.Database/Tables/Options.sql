CREATE TABLE [dbo].[Options]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [LarsCode] INT NOT NULL, 
    [IfaStdCode] NVARCHAR(10) NOT NULL, 
    [StandardName] NVARCHAR(250) NOT NULL, 
    [OptionName] NVARCHAR(MAX) NOT NULL
)

GO

CREATE INDEX [IX_Options_LarsCode] ON [dbo].[Options] ([LarsCode])
