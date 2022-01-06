
CREATE TABLE [OrganisationStandardVersion]
(
	[StandardUId] VARCHAR(20) NOT NULL ,
	[Version] NVARCHAR(20) NULL,    
	[OrganisationStandardId] INT NOT NULL, 
	[EffectiveFrom] DATETIME NULL,
	[EffectiveTo] DATETIME NULL,
	[DateVersionApproved] [DateTime] NULL,
	[Comments] [NVARCHAR] (500) NULL,
	[Status] [nvarchar](10) NOT NULL,
	) 
GO


CREATE UNIQUE INDEX IXU_OrganisationStandardVersion
   ON [OrganisationStandardVersion] ([OrganisationStandardId], [StandardUId], [Version]);

