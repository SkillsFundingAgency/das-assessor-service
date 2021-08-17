CREATE FUNCTION [dbo].[GetVersionFromLarsCode]
(
	@StartDate DATE,
	@StdCode Int
)
RETURNS NVARCHAR(12)
AS 
BEGIN
DECLARE @Result NVARCHAR(12) 
SET @Result = (
SELECT Version FROM (
SELECT row_number() OVER (PARTITION BY Ifatereferencenumber ORDER BY [dbo].[Expandedversion](version)) seq, * FROM Standards WHERE larscode = @StdCode 
AND (VersionLatestStartDate IS NULL OR VersionLatestStartDate >= @StartDate)
) st1 WHERE seq = 1)
RETURN @Result

END
GO