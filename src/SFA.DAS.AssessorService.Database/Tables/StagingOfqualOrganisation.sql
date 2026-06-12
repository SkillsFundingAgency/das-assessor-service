CREATE TABLE [dbo].[StagingOfqualOrganisation]
(
	[RecognitionNumber] [varchar] (10) null,
	[Name] [nvarchar] (100) null,
	[LegalName] [nvarchar] (200) null,
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
	[OfqualStatus] [nvarchar] (30) null,
	[OfqualRecognisedFrom] [datetime] null,
	[OfqualRecognisedTo] [datetime] null,
)
