CREATE TABLE [ao].[Standard]
(
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[StandardCode] NVARCHAR(10) NOT NULL,
	[Version] INT NULL,
	[StandardName] [NVARCHAR] (256) NOT NULL,
	[StandardSectorCode] INT NULL,
	[NotionalEndLevel] INT NULL,
	[EffectiveFrom] DATETIME NULL,
	[EffectiveTo] DATETIME NULL,
	[LastDateStarts] DATETIME NULL,
	[UrlLink] [NVARCHAR] (256) NULL,
	SectorSubjectAreaTier1 INT NULL,
	SectorSubjectAreaTier2 [NVARCHAR] (10) NULL, 
    [IntegratedDegreeStandard] BIT NULL, 
	[StatusId] INT NOT NULL,
    [CreatedOn] DATETIME NULL, 
    [CreatedBy] NVARCHAR(50) NULL, 
    [ModifiedOn] DATETIME NULL, 
    [ModifiedBy] NVARCHAR(50) NULL
)

GO

ALTER TABLE [ao].[Standard]
  ADD CONSTRAINT FK_StandardStatusId
  FOREIGN KEY (StatusId) REFERENCES [ao].[Status] (ID);
GO

CREATE INDEX IX_standardStandardCode
   ON [ao].[Standard] (StandardCode);
   
