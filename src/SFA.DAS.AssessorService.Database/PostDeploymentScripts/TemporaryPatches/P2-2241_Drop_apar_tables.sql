IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[APARSummary]') AND type in (N'U'))
DROP TABLE [dbo].[APARSummary]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[APARSummaryUpdated]') AND type in (N'U'))
DROP TABLE [dbo].[APARSummaryUpdated]
GO

