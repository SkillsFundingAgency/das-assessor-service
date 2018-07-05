CREATE TABLE [ao].[Standard]
(
	[StandardCode] INT NOT NULL PRIMARY KEY,
	[Version] INT NOT NULL,
	[StandardName] [NVARCHAR] (256) NOT NULL,
	[StandardSectorCode] INT NOT NULL,
	[NotionalEndLevel] INT NOT NULL,
	[EffectiveFrom] DATETIME NOT NULL,
	[EffectiveTo] DATETIME NULL,
	[LastDateStarts] DATETIME NULL,
	[UrlLink] [NVARCHAR] (256) NULL,
	SectorSubjectAreaTier1 INT NULL,
	SectorSubjectAreaTier2 [NVARCHAR] (10) NULL, 
    [IntegratedDegreeStandard] BIT NULL, 
    [CreatedOn] DATETIME NULL, 
    [CreatedBy] NVARCHAR(50) NULL, 
    [ModifiedOn] DATETIME NULL, 
    [ModifiedBy] NVARCHAR(50) NULL,


)
