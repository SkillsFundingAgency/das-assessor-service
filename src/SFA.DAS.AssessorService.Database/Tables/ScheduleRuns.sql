CREATE TABLE [dbo].[ScheduleRuns]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [RunTime] DATETIME2 NOT NULL, 
    [IsComplete] BIT NOT NULL DEFAULT 0, 
    [Interval] BIGINT NULL, 
    [IsRecurring] BIT NOT NULL DEFAULT 0, 
    [ScheduleType] INT NOT NULL
)
