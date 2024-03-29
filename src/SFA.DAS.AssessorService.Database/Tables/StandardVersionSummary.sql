-- Opportunity Finder data

CREATE TABLE [StandardVersionSummary](
    [StandardCode] INT NULL, 
    [StandardReference] [nvarchar](10) NULL,
    [Version] VARCHAR(20) NULL,
    [ActiveApprentices] INT NULL, 
    [CompletedAssessments] INT NULL, 
    [EndPointAssessors] INT NULL, 
    [UpdatedAt] datetime NULL,
) 
GO


CREATE INDEX [IX_StandardVersionSummary] ON [StandardVersionSummary] ([StandardReference])
GO

