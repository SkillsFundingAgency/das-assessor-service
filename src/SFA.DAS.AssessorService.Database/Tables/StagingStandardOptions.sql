CREATE TABLE [dbo].[StagingStandardOptions]
(
    [StandardUId] VARCHAR(20) NOT NULL,
    [OptionName] NVARCHAR(500) NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_StagingStandardOptions]
    ON [dbo].[StagingStandardOptions] ([StandardUId], [OptionName]);
