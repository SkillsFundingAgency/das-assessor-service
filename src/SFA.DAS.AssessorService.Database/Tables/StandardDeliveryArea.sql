
CREATE TABLE [ao].[StandardDeliveryArea]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[StandardCode] [INT] NOT NULL, 
	[DeliveryAreaId] [UNIQUEIDENTIFIER] NOT NULL,
	[Comments] [NVARCHAR] (500) NULL,
	CONSTRAINT [PK_StandardDeliveryArea] PRIMARY KEY ([Id]),
	) ON [PRIMARY] 
GO

	
ALTER TABLE [ao].[StandardDeliveryArea]
ADD CONSTRAINT FK_StandardStandardDeliveryArea
FOREIGN KEY (StandardCode) REFERENCES [ao].[Standard] (StandardCode);

GO

ALTER TABLE [ao].[StandardDeliveryArea]
ADD CONSTRAINT FK_DeliveryAreaIdStandardDeliveryArea
FOREIGN KEY (DeliveryAreaId) REFERENCES [ao].[DeliveryArea] (Id);

