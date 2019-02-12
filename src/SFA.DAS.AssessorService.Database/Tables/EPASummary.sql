CREATE TABLE [dbo].[EPASummary]
	(
		[EndPointAssessorOrganisationId] nvarchar(12) NOT NULL,
		[StandardCode] int NOT NULL,
		[ReportedData] nvarchar(max),
		[ReportedTo] date NULL,
		[CreatedAt] datetime DEFAULT GETDATE(),
		[UpdatedAt] datetime NULL
	)
GO
	
CREATE UNIQUE INDEX [IXU_EPASummary_EPAO_StdCode] ON EPASummary ([EndPointAssessorOrganisationId], [StandardCode]);


