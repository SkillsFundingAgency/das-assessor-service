﻿CREATE FUNCTION [dbo].[GetVersionFromLarsCode]
(
	@StartDate DATE,
	@StdCode Int
)
RETURNS NVARCHAR(12)
AS 
BEGIN
	DECLARE @Result NVARCHAR(12) 
	SET @Result = 
	(
		SELECT Version FROM 
		(
			SELECT 
				ROW_NUMBER() OVER (PARTITION BY IFateReferenceNumber ORDER BY CASE 
					WHEN @StartDate BETWEEN ISNULL(VersionEarliestStartDate, [dbo].GetMinDateTime()) AND ISNULL(VersionLatestStartDate, [dbo].GetMaxDateTime()) THEN 0 
					ELSE 1 END, VersionMajor DESC, VersionMinor DESC) Seq, 
				[Version]
			FROM 
				Standards 
			WHERE 
				LarsCode = @StdCode 
				AND VersionApprovedForDelivery IS NOT NULL
		) [AllValidVersions] 
		WHERE 
			Seq = 1
	)

	RETURN @Result
END
GO


