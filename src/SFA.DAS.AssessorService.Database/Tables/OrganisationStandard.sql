CREATE TABLE [OrganisationStandard]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	EndPointAssessorOrganisationId [nvarchar](12) NOT NULL, 
	[StandardCode] INT NOT NULL,
	[EffectiveFrom] [DateTime] NULL,
	[EffectiveTo] [DateTime] NULL,
	[DateStandardApprovedOnRegister] [DateTime] NULL,
	[Comments] [NVARCHAR] (500) NULL,
	[Status] [nvarchar](10) NOT NULL,
) ON [PRIMARY]

GO

CREATE INDEX IX_standardOrgIdStandardCode
   ON [OrganisationStandard] (EndPointAssessorOrganisationId, StandardCode);

GO

ALTER TABLE [OrganisationStandard]
ADD CONSTRAINT FK_OrganisationIdentifierStandard
FOREIGN KEY (EndPointAssessorOrganisationId) REFERENCES [Organisations] ([EndPointAssessorOrganisationId]);
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_standardOrgIdStandardCodeEffectiveFrom
   ON [OrganisationStandard] (EndPointAssessorOrganisationId, StandardCode, EffectiveFrom);   