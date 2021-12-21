
CREATE TABLE [MergeOrganisationStandardVersion]
(
	[StandardUId] VARCHAR(20) NOT NULL ,
	[Version] NVARCHAR(20) NULL,    
	[OrganisationStandardId] INT NOT NULL, 
	[EffectiveFrom] DATETIME NULL,
	[EffectiveTo] DATETIME NULL,
	[DateVersionApproved] [DateTime] NULL,
	[Comments] [NVARCHAR] (500) NULL,
	[Status] [nvarchar](10) NOT NULL,
	[MergeOrganisationId] int NOT NULL,
	[Replicates] NVARCHAR (6) NOT NULL
	) 
GO


CREATE UNIQUE INDEX IXU_OrganisationStandardVersion
   ON [MergeOrganisationStandardVersion] ([OrganisationStandardId], [StandardUId]);
GO


ALTER TABLE [MergeOrganisationStandardVersion]
ADD CONSTRAINT FK_MergeOrganisationStandardVersionMergeId
FOREIGN KEY (MergeOrganisationId) REFERENCES [MergeOrganisations] ([Id]);

GO

