CREATE TABLE [dbo].[OrganisationDeliveryArea]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[EndPointAssessorOrganisationId] [nvarchar](12) NOT NULL, 
	[DeliveryAreaId] [int] NOT NULL,
	[Comments] [nvarchar] (500) NULL,
	[Status] [nvarchar](10) NOT NULL,
	) ON [PRIMARY] 
GO

ALTER TABLE [OrganisationDeliveryArea]
ADD CONSTRAINT FK_OrganisationDeliveryArea_DeliveryAreaId
FOREIGN KEY (DeliveryAreaId) REFERENCES [DeliveryArea] (Id);
GO

ALTER TABLE [OrganisationDeliveryArea]
ADD CONSTRAINT FK_OrganisationDeliveryArea_EndPointAssessorOrganisationId
FOREIGN KEY (EndPointAssessorOrganisationId) REFERENCES [Organisations] ([EndPointAssessorOrganisationId]);
GO

CREATE UNIQUE INDEX IXU_OrganisationDeliveryArea_EndPointAssessorOrganisationId_DeliveryAreaId
   ON [OrganisationDeliveryArea] ([EndPointAssessorOrganisationId], [DeliveryAreaId]);
GO
   
