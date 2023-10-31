CREATE TABLE [dbo].[APARSummary]
(
[EndPointAssessorOrganisationId] nvarchar(12) NOT NULL,
[EndPointAssessorName] nvarchar(100) NOT NULL,
[EndPointAssessorUkprn] int NOT NULL,
[EarliestDateStandardApprovedOnRegister] datetime NOT NULL,
[EarliestEffectiveFromDate] datetime NOT NULL,
CONSTRAINT [PK_APARSummary] PRIMARY KEY CLUSTERED ([EndPointAssessorOrganisationId])
);
Go

CREATE UNIQUE INDEX [IXU_APARSummary] ON [dbo].[APARSummary] ([EndPointAssessorUkprn]) 
INCLUDE ([EndPointAssessorOrganisationId],[EndPointAssessorName],[EarliestDateStandardApprovedOnRegister],[EarliestEffectiveFromDate]);
GO

