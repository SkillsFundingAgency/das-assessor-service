CREATE TABLE [ao].[EpaOrganisationStandard]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[EPAOrganisationIdentifier] [nvarchar](7) NOT NULL, 
	[StandardCode] NVARCHAR(10) NOT NULL,
	[EffectiveFrom] [DateTime] NULL,
	[EffectiveTo] [DateTime] NULL,
	[ContactName] [nvarchar](200) NULL,
	[ContactPhoneNumber] [nvarchar] (50) NULL,
	[ContactEmail] [nvarchar] (200) NULL, 
	[DateStandardApprovedOnRegister] [DateTime] NULL,
	[Comments] [NVARCHAR] (500) NULL,
	[StatusId] INT NOT NULL, 
    CONSTRAINT [PK_Standard] PRIMARY KEY ([Id]),
) ON [PRIMARY]

GO


ALTER TABLE [ao].[EpaOrganisationStandard]
ADD CONSTRAINT FK_OrganisationIdentifierStandard
FOREIGN KEY (EPAOrganisationIdentifier) REFERENCES [ao].[EPAOrganisation] (EPAOrganisationIdentifier);

GO

CREATE INDEX IX_standardOrgIdStandardCode
   ON [ao].[EpaOrganisationStandard] (EPAOrganisationIdentifier, StandardCode);

GO


CREATE UNIQUE NONCLUSTERED INDEX IX_standardOrgIdStandardCodeEffectiveFrom
   ON [ao].[EpaOrganisationStandard] (EPAOrganisationIdentifier, StandardCode, EffectiveFrom);   