
CREATE TABLE [ao].[StandardDeliveryArea]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[EPAOrganisationIdentifier] [nvarchar](7) NOT NULL, 
	[StandardCode] [int] NOT NULL,
	[DeliveryAreaId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_StandardDeliveryArea] PRIMARY KEY ([Id]),
	) ON [PRIMARY] 
GO

	
ALTER TABLE [ao].[StandardDeliveryArea]
ADD CONSTRAINT FK_StandardStandardDeliveryArea
FOREIGN KEY (EPAOrganisationIdentifier,StandardCode) REFERENCES [ao].[Standard] (EPAOrganisationIdentifier, StandardCode);

GO

ALTER TABLE [ao].[StandardDeliveryArea]
ADD CONSTRAINT FK_DeliveryAreaIdStandardDeliveryArea
FOREIGN KEY (DeliveryAreaId) REFERENCES [ao].[DeliveryArea] (Id);

