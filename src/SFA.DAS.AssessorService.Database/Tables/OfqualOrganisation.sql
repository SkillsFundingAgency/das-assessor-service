CREATE TABLE [dbo].[OfqualOrganisation]
(
	[Id] [uniqueidentifier] not null DEFAULT NEWID(),
	[RecognitionNumber] [varchar] (10) not null,
	[Name] [nvarchar] (100) not null,
	[LegalName] [nvarchar] (200) not null,
	[Acronym] [nvarchar] (100) null,
	[Email] [nvarchar] (256) null,
	[Website] [nvarchar] (2000) null,
	[HeadOfficeAddressLine1] [nvarchar] (200) null,
	[HeadOfficeAddressLine2] [nvarchar] (200) null,
	[HeadOfficeAddressTown] [nvarchar] (200) null,
	[HeadOfficeAddressCounty] [nvarchar] (200) null,
	[HeadOfficeAddressPostcode] [nvarchar] (10) null,
	[HeadOfficeAddressCountry] [nvarchar] (100) null,
	[HeadOfficeAddressTelephone] [nvarchar] (50) null,
	[OfqualStatus] [nvarchar] (30) not null,
	[OfqualRecognisedFrom] [datetime] not null,
	[OfqualRecognisedTo] [datetime] null,
	[CreatedAt] [datetime] not null DEFAULT GETUTCDATE(),
	[UpdatedAt] [datetime] null,
 CONSTRAINT [PK_OfqualOrganisation] PRIMARY KEY CLUSTERED ( [Id] ASC )    
)
GO

CREATE UNIQUE INDEX [IXU_OfqualOrganisation_RecognitionNumber] ON [OfqualOrganisation] ([RecognitionNumber])
GO

