CREATE TABLE [dbo].[ScheduleConfigurations](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Data] [nvarchar](50) NOT NULL	
    CONSTRAINT [PK_ScheduleConfigurations] PRIMARY KEY ([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

