CREATE TABLE [dbo].[AssessmentsSummary]
([Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
 [Ukprn] bigint NOT NULL,
 [IfateReferenceNumber] nvarchar(10) NOT NULL,
 [EarliestAssessment] datetime2  NOT NULL, 
 [EndpointAssessmentCount] int NOT NULL, 
 [UpdatedAt] datetime2);
 GO
 
 CREATE UNIQUE INDEX IXU_AssessmentsSummary ON [dbo].[AssessmentsSummary]
 ([Ukprn],[IfateReferenceNumber])
 INCLUDE ([EarliestAssessment], [EndpointAssessmentCount]);
 GO
 
 CREATE INDEX IX_AssessmentsSummary_Ukprn ON [dbo].[AssessmentsSummary]
 ([Ukprn])
 INCLUDE ([EarliestAssessment], [EndpointAssessmentCount]);
 GO
 