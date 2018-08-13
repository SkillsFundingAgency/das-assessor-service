﻿
CREATE TABLE [OrganisationStandardDeliveryArea]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[OrganisationStandardId] INT NOT NULL, 
	[DeliveryAreaId] [int] NOT NULL,
	[Comments] [NVARCHAR] (500) NULL,
	[Status] [nvarchar](10) NOT NULL,
	) ON [PRIMARY] 
GO


ALTER TABLE [OrganisationStandardDeliveryArea]
ADD CONSTRAINT FK_DeliveryAreaIdStandardDeliveryArea
FOREIGN KEY (DeliveryAreaId) REFERENCES [DeliveryArea] (Id);
GO

ALTER TABLE [OrganisationStandardDeliveryArea]
ADD CONSTRAINT FK_OrganisationStandardId
FOREIGN KEY ([OrganisationStandardId]) REFERENCES [OrganisationStandard] ([Id]);

GO

CREATE UNIQUE INDEX IX_standardDeliveryAreaCoveredIndex
   ON [OrganisationStandardDeliveryArea] ([OrganisationStandardId], [DeliveryAreaId]);

