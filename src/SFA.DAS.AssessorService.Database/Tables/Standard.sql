CREATE TABLE [ao].[Standard]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[EPAOrganisationIdentifier] [nvarchar](7) NOT NULL, 
	[StandardCode] [int] NOT NULL,
	[EffectiveFrom] [DateTime] NOT NULL,
	[EffectiveTo] [DateTime] NULL,
	[ContactName] [nvarchar](200) NULL,
	[ContactPhoneNumber] [nvarchar] (20) NULL,
	[ContactEmail] [nvarchar] (200) NULL, 
	[DateStandardApproviedOnRegister] [DateTime] NULL,
	CONSTRAINT [PK_Standard] PRIMARY KEY ([Id]),
) ON [PRIMARY]

GO


ALTER TABLE [ao].[Standard]
ADD CONSTRAINT FK_OrganisationIdentifierStandard
FOREIGN KEY (EPAOrganisationIdentifier) REFERENCES [ao].[EPAOrganisation] (EPAOrganisationIdentifier);

GO

CREATE UNIQUE NONCLUSTERED INDEX IX_standardOrgIdStandardCode
   ON [ao].[Standard] (EPAOrganisationIdentifier, StandardCode);
   