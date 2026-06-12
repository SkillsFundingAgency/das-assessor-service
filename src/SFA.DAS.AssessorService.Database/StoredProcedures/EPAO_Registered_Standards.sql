CREATE PROCEDURE [dbo].[EPAO_Registered_Standards]
	 @EPAOID AS NVARCHAR(12),
	 @RequireAtLeastOneVersion AS INT,
	 @Skip int,
	 @Take int
AS
BEGIN
WITH LatestVersions
AS
(
	SELECT IFateReferenceNumber, Title, Version, [dbo].[ExpandedVersion](Version) MaxVersion, Level, LarsCode
	FROM (
	SELECT IFateReferenceNumber, Title, Version, Level, LarsCode, ROW_NUMBER() OVER (PARTITION BY IFateReferenceNumber ORDER BY [dbo].[ExpandedVersion](Version) DESC) rownumber FROM Standards
	WHERE VersionApprovedForDelivery IS NOT NULL
	) sv1 WHERE rownumber = 1
)
SELECT 
	lv.Larscode as StandardCode,
	lv.Title as [StandardName],
	lv.[Level],
	lv.IFateReferenceNumber as ReferenceNumber,
	IIF(lv.MaxVersion > ISNULL(osv.MaxVersion, 0), 1, 0) AS NewVersionAvailable,
	ISNULL(osv.NumberOfVersions, 0)  AS NumberOfVersions 
FROM OrganisationStandard os 
	LEFT JOIN (
		SELECT OrganisationStandardId,  MAX( [dbo].[ExpandedVersion]([Version]) ) AS MaxVersion, Count(*) AS NumberOfVersions 
		FROM OrganisationStandardVersion 
		WHERE EffectiveTo is null OR EffectiveTo > GETDATE()
		GROUP BY OrganisationStandardId
	) AS osv ON osv.OrganisationStandardId = os.Id
	INNER JOIN Organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.EndPointAssessorOrganisationId = @EPAOID
	INNER JOIN LatestVersions lv on lv.IFateReferenceNumber = os.StandardReference
	WHERE os.Status = 'Live' 
	AND (os.EffectiveTo is null OR os.EffectiveTo > GETDATE())
	AND (@RequireAtLeastOneVersion = 0 OR ISNULL(osv.NumberOfVersions, 0) > 0)
	ORDER BY lv.Title
	OFFSET @Skip ROWS 
	FETCH NEXT @Take ROWS ONLY
END
GO
