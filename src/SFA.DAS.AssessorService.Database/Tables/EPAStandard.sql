CREATE TABLE [ao].[EpaStandard]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[EPAOrganisationIdentifier] [nvarchar](7) NOT NULL, 
	[StandardCode] [int] NOT NULL,
	[EffectiveFrom] [DateTime] NOT NULL,
	[EffectiveTo] [DateTime] NULL,
	[ContactName] [nvarchar](200) NULL,
	[ContactPhoneNumber] [nvarchar] (50) NULL,
	[ContactEmail] [nvarchar] (200) NULL, 
	[DateStandardApprovedOnRegister] [DateTime] NULL,
	[Comments] [NVARCHAR] (500) NULL,
	CONSTRAINT [PK_Standard] PRIMARY KEY ([Id]),
) ON [PRIMARY]

GO


ALTER TABLE [ao].[EpaStandard]
ADD CONSTRAINT FK_OrganisationIdentifierStandard
FOREIGN KEY (EPAOrganisationIdentifier) REFERENCES [ao].[EPAOrganisation] (EPAOrganisationIdentifier);

GO

CREATE INDEX IX_standardOrgIdStandardCode
   ON [ao].[EpaStandard] (EPAOrganisationIdentifier, StandardCode);

GO


CREATE UNIQUE NONCLUSTERED INDEX IX_standardOrgIdStandardCodeEffectiveFrom
   ON [ao].[EpaStandard] (EPAOrganisationIdentifier, StandardCode, EffectiveFrom);   