CREATE TABLE [OrganisationStandard]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	EndPointAssessorOrganisationId [nvarchar](12) NOT NULL, 
	[EffectiveFrom] [DateTime] NULL,
	[EffectiveTo] [DateTime] NULL,
	[DateStandardApprovedOnRegister] [DateTime] NULL,
	[Comments] [NVARCHAR] (500) NULL,
	[Status] [nvarchar](10) NOT NULL, 
	[ContactId] [UNIQUEIDENTIFIER] NULL,
    OrganisationStandardData [nvarchar](max) NULL,
    [StandardReference] NVARCHAR(10) NULL ,
) ON [PRIMARY]

GO


ALTER TABLE [OrganisationStandard]
ADD CONSTRAINT FK_OrganisationIdentifierStandard
FOREIGN KEY (EndPointAssessorOrganisationId) REFERENCES [Organisations] ([EndPointAssessorOrganisationId]);
GO

GO
CREATE UNIQUE NONCLUSTERED INDEX IX_standardOrgIdStandardReferenceEffectiveFrom
   ON [OrganisationStandard] ([EndPointAssessorOrganisationId], [StandardReference])
   INCLUDE ([EffectiveFrom], [EffectiveTo], [Status]);   

GO

ALTER TABLE [OrganisationStandard]
ADD CONSTRAINT FK_OrganisationContact
FOREIGN KEY (ContactId) REFERENCES Contacts ([Id]);
GO
