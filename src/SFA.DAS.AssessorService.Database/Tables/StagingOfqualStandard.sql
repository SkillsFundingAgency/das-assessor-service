CREATE TABLE [dbo].[StagingOfqualStandard]
(
	[RecognitionNumber] [varchar] (10) not null,
	[OperationalStartDate] [datetime] not null,
	[OperationalEndDate] [datetime] null,
	[IfateReferenceNumber] [varchar] (32) not null,
)
