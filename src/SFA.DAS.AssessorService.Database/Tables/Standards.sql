CREATE TABLE [dbo].[Standards]
(
	[StandardUId] VARCHAR(20) NOT NULL PRIMARY KEY, 
	[IFateReferenceNumber] VARCHAR(10) NOT NULL,
	[LarsCode] INT NULL,
	[Title] VARCHAR(1000) NOT NULL,
	[Version] DECIMAL(18,1) NULL,
	[Level] INT NOT NULL,
	[Status] VARCHAR(100) NOT NULL,
	[TypicalDuration] INT NOT NULL,
	[MaxFunding] INT NOT NULL,
	[IsActive] BIT NOT NULL,
	[LastDateStarts] DATETIME NULL,
	[EffectiveFrom] DATETIME NULL,
	[EffectiveTo] DATETIME NULL, 
    [VersionEarliestStartDate] DATETIME NULL, 
    [VersionLatestStartDate] DATETIME NULL, 
    [VersionLatestEndDate] DATETIME NULL, 
    [VersionApprovedForDelivery] DATETIME NULL, 
    [ProposedTypicalDuration] INT NOT NULL, 
    [ProposedMaxFunding] INT NOT NULL,
	[EPAChanged] BIT NOT NULL,
	[StandardPageUrl] VARCHAR(500) NULL
)
