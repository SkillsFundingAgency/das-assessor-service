
CREATE TABLE [ao].[EpaOrganisationStandardDeliveryArea]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[EPAOrganisationIdentifier] [nvarchar](7) NOT NULL,
	[StandardCode] NVARCHAR(10) NOT NULL, 
	[DeliveryAreaId] [int] NOT NULL,
	[Comments] [NVARCHAR] (500) NULL,
	) ON [PRIMARY] 
GO

ALTER TABLE [ao].[EpaOrganisationStandardDeliveryArea]
ADD CONSTRAINT FK_DeliveryAreaIdStandardDeliveryArea
FOREIGN KEY (DeliveryAreaId) REFERENCES [ao].[DeliveryArea] (Id);

GO

CREATE UNIQUE INDEX IX_standardDeliveryAreaCoveredIndex
   ON [ao].[EpaOrganisationStandardDeliveryArea] ([EPAOrganisationIdentifier], [StandardCode], [DeliveryAreaId]);

