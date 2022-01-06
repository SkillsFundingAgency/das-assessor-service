CREATE TABLE [MergeOrganisationStandard]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	EndPointAssessorOrganisationId [nvarchar](12) NOT NULL, 
	[StandardCode] INT NOT NULL,
	[EffectiveFrom] [DateTime] NULL,
	[EffectiveTo] [DateTime] NULL,
	[DateStandardApprovedOnRegister] [DateTime] NULL,
	[Comments] [NVARCHAR] (500) NULL,
	[Status] [nvarchar](10) NOT NULL, 
	[ContactId] [UNIQUEIDENTIFIER] NULL,
    OrganisationStandardData [nvarchar](max) NULL,
    [StandardReference] NVARCHAR(10) NULL ,
	[MergeOrganisationId] int NOT NULL,
	[Replicates] NVARCHAR (6) NOT NULL
) ON [PRIMARY]

GO


ALTER TABLE [MergeOrganisationStandard]
ADD CONSTRAINT FK_MergeOrganisationIdentifierStandard
FOREIGN KEY (EndPointAssessorOrganisationId) REFERENCES [Organisations] ([EndPointAssessorOrganisationId]);

GO


ALTER TABLE [MergeOrganisationStandard]
ADD CONSTRAINT FK_MergeOrganisationId
FOREIGN KEY (MergeOrganisationId) REFERENCES [MergeOrganisations] ([Id]);

GO
