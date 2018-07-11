CREATE TABLE [ao].[EPAOrganisation]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[EPAOrganisationIdentifier] [nvarchar](7) NOT NULL, 
	[EPAOrganisationName] [nvarchar](256) NOT NULL,
	[OrganisationTypeId] [int] NULL,   
	[WebsiteLink] [nvarchar](256) NULL,
	[ContactAddress1] [nvarchar](50) NULL,
	[ContactAddress2] [nvarchar](50) NULL,
	[ContactAddress3] [nvarchar](50) NULL,
	[ContactAddress4] [nvarchar](50) NULL,
	[ContactPostcode] [nvarchar](8) NULL,
	[UKPRN] [int] NULL,
	[LegalName] [nvarchar](256) NULL,
	[StatusId] int NOT NULL,
	CONSTRAINT IX_EPAOrganisationIdentifierOrganisation UNIQUE (EPAOrganisationIdentifier),
	INDEX IX_OrganisationTypeId  (OrganisationTypeId),
) ON [PRIMARY] 
GO


ALTER TABLE [ao].[EPAOrganisation]
ADD CONSTRAINT FK_OrganisationType
FOREIGN KEY (OrganisationTypeId) REFERENCES [ao].[OrganisationType] (ID);

GO

ALTER TABLE [ao].[EPAOrganisation]
ADD CONSTRAINT FK_OrganisationStatusId
FOREIGN KEY (StatusId) REFERENCES [ao].[Status] (Id);
