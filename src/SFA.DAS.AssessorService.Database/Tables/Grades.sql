CREATE TABLE [Grades](
	[GradeId] [nvarchar](50) NOT NULL,
	[Grade] [nvarchar](50) NOT NULL,
	[Pass] [bit] NOT NULL DEFAULT(1),
	[Enabled] [bit] NOT NULL DEFAULT(1),
 CONSTRAINT [PK_Grades] PRIMARY KEY  
 (
	[GradeId]
 )
)
GO