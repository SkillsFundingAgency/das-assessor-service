
CREATE TABLE [ao].[EpaOrganisationStandardDeliveryArea]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[EPAOrganisationIdentifier] [nvarchar](7) NOT NULL,
	[StandardCode] [INT] NOT NULL, 
	[DeliveryAreaId] [UNIQUEIDENTIFIER] NOT NULL,
	[Comments] [NVARCHAR] (500) NULL,
	CONSTRAINT [PK_StandardDeliveryArea] PRIMARY KEY ([Id]),
	) ON [PRIMARY] 
GO

ALTER TABLE [ao].[EpaOrganisationStandardDeliveryArea]
ADD CONSTRAINT FK_DeliveryAreaIdStandardDeliveryArea
FOREIGN KEY (DeliveryAreaId) REFERENCES [ao].[DeliveryArea] (Id);

GO

CREATE UNIQUE INDEX IX_standardDeliveryAreaCoveredIndex
   ON [ao].[EpaOrganisationStandardDeliveryArea] ([EPAOrganisationIdentifier], [StandardCode], [DeliveryAreaId]);

