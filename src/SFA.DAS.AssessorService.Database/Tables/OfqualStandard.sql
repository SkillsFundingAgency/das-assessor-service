CREATE TABLE [dbo].[OfqualStandard]
(
	[Id] [uniqueidentifier] not null DEFAULT NEWID(),
	[RecognitionNumber] [varchar] (10) not null,
	[OperationalStartDate] [datetime] not null,
	[OperationalEndDate] [datetime] null,
	[IfateReferenceNumber] [varchar] (10) not null,
	[CreatedAt] [datetime] not null DEFAULT GETUTCDATE(),
	[UpdatedAt] [datetime] null,
 CONSTRAINT [PK_OfqualStandard] PRIMARY KEY CLUSTERED  ( [Id] ASC )  
)
GO

CREATE UNIQUE INDEX [IXU_OfqualStandard_RecognitionNumber_IfateReferenceNumber] ON [OfqualStandard] ([RecognitionNumber], [IfateReferenceNumber])
GO

