CREATE TABLE [ao].[DeliveryArea](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Area] [nvarchar](256) NOT NULL,
	CONSTRAINT [PK_DeliveryAreas] PRIMARY KEY ([Id]),
) ON [PRIMARY] 
