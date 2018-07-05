
CREATE TABLE [ao].[StandardDeliveryArea]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[StandardId] [uniqueidentifier] NOT NULL, 
	[DeliveryAreaId] [UNIQUEIDENTIFIER] NOT NULL,
	[Comments] [NVARCHAR] (500) NULL,
	CONSTRAINT [PK_StandardDeliveryArea] PRIMARY KEY ([Id]),
	) ON [PRIMARY] 
GO

	
ALTER TABLE [ao].[StandardDeliveryArea]
ADD CONSTRAINT FK_StandardStandardDeliveryArea
FOREIGN KEY (StandardId) REFERENCES [ao].[Standard] (Id);

GO

ALTER TABLE [ao].[StandardDeliveryArea]
ADD CONSTRAINT FK_DeliveryAreaIdStandardDeliveryArea
FOREIGN KEY (DeliveryAreaId) REFERENCES [ao].[DeliveryArea] (Id);

