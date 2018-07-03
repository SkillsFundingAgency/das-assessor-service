CREATE TABLE [ao].[EPAOrganisation]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[EPAOrganisationIdentifier] [nvarchar](7) NOT NULL, 
	[EPAOrganisationName] [nvarchar](256) NOT NULL,
	[OrganisationTypeId] [uniqueidentifier] NOT NULL,   
	[WebsiteLink] [nvarchar](256) NULL,
	[ContactAddress1] [nvarchar](50) NULL,
	[ContactAddress2] [nvarchar](50) NULL,
	[ContactAddress3] [nvarchar](50) NULL,
	[ContactAddress4] [nvarchar](50) NULL,
	[ContactPostcode] [nvarchar](8) NULL,
	[UKPRN] [int] NULL,
	[LegalName] [nvarchar](256) NULL,
	CONSTRAINT [PK_Organisation] PRIMARY KEY ([Id]),
	CONSTRAINT IX_EPAOrganisationIdentifierOrganisation UNIQUE (EPAOrganisationIdentifier),
	INDEX IX_OrganisationTypeId  (OrganisationTypeId),
) ON [PRIMARY] 
GO




ALTER TABLE [ao].[EPAOrganisation]
ADD CONSTRAINT FK_OrganisationType
FOREIGN KEY (OrganisationTypeId) REFERENCES [ao].[OrganisationType] (ID);
