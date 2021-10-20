-- Opportunity Finder data

CREATE TABLE [StandardSummary](
    [StandardCode] INT NULL, 
    [StandardReference] [nvarchar](10) NULL,
    [StandardName] [nvarchar](500) NOT NULL,
    [StandardLevel] INT NULL, 
    [Sector] [nvarchar](250) NOT NULL,
    [Region] [nvarchar](50) NOT NULL,
    [Ordering] INT NULL, 
    [Learners] INT NULL, 
    [Assessments] INT NULL, 
    [TotalEPAOs] INT NULL, 
    [EndPointAssessors] INT NULL, 
    [EndPointAssessorList] [nvarchar](max) NOT NULL,
    [UpdatedAt] datetime NULL,
    [Versions] [nvarchar](500) NULL
) 
GO


CREATE INDEX [IX_StandardSummary] ON [StandardSummary] ([StandardReference],[Ordering])
GO

