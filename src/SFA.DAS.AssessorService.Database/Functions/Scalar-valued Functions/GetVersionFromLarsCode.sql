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
SELECT row_number() OVER (PARTITION BY IFateReferenceNumber ORDER BY VersionMajor, VersionMinor) seq, * FROM Standards WHERE LarsCode = @StdCode 
AND (VersionLatestStartDate IS NULL OR VersionLatestStartDate >= @StartDate)
) st1 WHERE seq = 1)
RETURN @Result

END
GO