CREATE TABLE [dbo].[OfqualStandard]
(
	[Id] [uniqueidentifier] not null DEFAULT NEWID(),
	[RecognitionNumber] [varchar] (10) not null,
	[OperationalStartDate] [datetime] not null,
	[OperationalEndDate] [datetime] null,
	[IFateReferenceNumber] [varchar] (10) not null,
	[CreatedAt] [datetime] not null DEFAULT GETUTCDATE(),
	[UpdatedAt] [datetime] null,
	[DeletedAt] [datetime] null,
 CONSTRAINT [PK_OfqualStandard] PRIMARY KEY CLUSTERED  ( [Id] ASC )  
)
GO

CREATE UNIQUE INDEX [IXU_OfqualOrganisation_RecognitionNumber] ON [OfqualStandard] ([RecognitionNumber])
GO

