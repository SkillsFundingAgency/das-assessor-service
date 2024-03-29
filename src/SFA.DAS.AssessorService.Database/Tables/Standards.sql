﻿CREATE TABLE [dbo].[Standards]
(
	[StandardUId] VARCHAR(20) NOT NULL PRIMARY KEY, 
	[IFateReferenceNumber] VARCHAR(10) NOT NULL,
	[LarsCode] INT NULL,
	[Title] VARCHAR(1000) NOT NULL,
	[Version] VARCHAR(20) NULL,
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
	[EPAChanged] BIT NOT NULL DEFAULT 0,
	[StandardPageUrl] VARCHAR(500) NULL, 
	[TrailBlazerContact] VARCHAR(500) NULL, 
	[Route] VARCHAR(500) NULL,
	[VersionMajor] INT NOT NULL DEFAULT 0,
	[VersionMinor] INT NOT NULL DEFAULT 0,
	[IntegratedDegree] VARCHAR(500) NULL,
	[EqaProviderName] VARCHAR(500) NULL,
	[EqaProviderContactName] VARCHAR(500) NULL,
	[EqaProviderContactEmail] VARCHAR(500) NULL,
	[OverviewOfRole] VARCHAR(500),
	[CoronationEmblem] BIT NOT NULL DEFAULT 0,
	[EpaoMustBeApprovedByRegulatorBody] BIT NOT NULL Default 0,
)
GO

CREATE NONCLUSTERED INDEX [IX_Standards_LarsCode_VersionLatestStartDate] ON [Standards] ([LarsCode],[VersionLatestStartDate])
GO
