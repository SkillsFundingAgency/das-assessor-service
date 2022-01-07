
CREATE TABLE [MergeOrganisationStandardDeliveryArea]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[OrganisationStandardId] INT NOT NULL, 
	[DeliveryAreaId] [int] NOT NULL,
	[Comments] [NVARCHAR] (500) NULL,
	[Status] [nvarchar](10) NOT NULL,
	[MergeOrganisationId] int NOT NULL,
	[Replicates] NVARCHAR (6) NOT NULL
	) ON [PRIMARY] 
GO


ALTER TABLE [MergeOrganisationStandardDeliveryArea]
ADD CONSTRAINT FK_MergeDeliveryAreaIdStandardDeliveryArea
FOREIGN KEY (DeliveryAreaId) REFERENCES [DeliveryArea] (Id);
GO

ALTER TABLE [MergeOrganisationStandardDeliveryArea]
ADD CONSTRAINT FK_MergeOrganisationStandardId
FOREIGN KEY ([OrganisationStandardId]) REFERENCES [OrganisationStandard] ([Id]);

GO

CREATE UNIQUE INDEX IX_standardMergeDeliveryAreaCoveredIndex
   ON [MergeOrganisationStandardDeliveryArea] ([Id], [OrganisationStandardId], [DeliveryAreaId]);
GO

ALTER TABLE [MergeOrganisationStandardDeliveryArea]
ADD CONSTRAINT FK_MergeOrganisationStandardMergeId
FOREIGN KEY (MergeOrganisationId) REFERENCES [MergeOrganisations] ([Id]);

GO