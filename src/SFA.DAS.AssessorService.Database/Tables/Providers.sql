CREATE TABLE [dbo].[Providers]
(
    [Ukprn] BIGINT NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL,
)
GO

CREATE UNIQUE INDEX [IXU_Providers] ON [Providers] ([Ukprn]) INCLUDE ([Name])
GO