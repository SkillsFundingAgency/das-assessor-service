CREATE PROCEDURE [dbo].[EPAO_Registered_Standards]
	 @EPAOID AS NVARCHAR(12),
	 @Skip int,
	 @Take int
AS
BEGIN
WITH LatestVersions
AS
(
	SELECT IFateReferenceNumber, Title, Version, [dbo].[ExpandedVersion](Version) MaxVersion, Level, Larscode
	FROM (
	SELECT IFateReferenceNumber, Title, Version, Level, Larscode, ROW_NUMBER() OVER (PARTITION BY IFateReferenceNumber ORDER BY [dbo].[ExpandedVersion](Version) DESC) rownumber FROM Standards
	WHERE VersionApprovedForDelivery IS NOT NULL
	) sv1 WHERE rownumber = 1
),
AppliedVersions AS
(
	SELECT StandardReference, MAX([dbo].[ExpandedVersion](v1.Version)) AS MaxVersion
	FROM Apply ap1
	CROSS APPLY OPENJSON(ApplyData, '$.Apply.Versions') WITH (version VARCHAR(10) '$') v1
	CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, NotRequired BIT) sequence
	JOIN Organisations og1 on og1.id = ap1.OrganisationId
	WHERE 1=1
	  AND sequence.NotRequired = 0
	  AND sequence.sequenceNo = [dbo].[ApplyConst_STANDARD_SEQUENCE_NO]() 
	  AND ap1.standardreference IS NOT NULL
	  AND ap1.ApplicationStatus NOT IN('Approved', 'Declined')
	  AND ap1.DeletedAt IS NULL
	  AND og1.EndPointAssessorOrganisationId = @EPAOID
	GROUP BY StandardReference
)
SELECT 
	lv.Larscode as StandardCode,
	lv.Title as [StandardName],
	lv.[Level],
	lv.IFateReferenceNumber as ReferenceNumber,
	IIF(lv.MaxVersion > osv.MaxVersion AND (av.MaxVersion IS NULL OR lv.MaxVersion > av.MaxVersion), 1, 0) AS NewVersionAvailable,
	osv.NumberOfVersions  AS NumberOfVersions 
FROM OrganisationStandard os 
	INNER JOIN (
		SELECT OrganisationStandardId,  MAX( [dbo].[ExpandedVersion]([Version]) ) AS MaxVersion, Count(*) AS NumberOfVersions 
		FROM OrganisationStandardVersion 
		WHERE EffectiveTo is null OR EffectiveTo > GETDATE()
		GROUP BY OrganisationStandardId
	) AS osv ON osv.OrganisationStandardId = os.Id
	INNER JOIN Organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.EndPointAssessorOrganisationId = @EPAOID
	INNER JOIN LatestVersions lv on lv.IFateReferenceNumber = os.StandardReference
	LEFT OUTER JOIN AppliedVersions av on av.StandardReference = os.StandardReference
	WHERE os.Status = 'Live' 
	AND (os.EffectiveTo is null OR os.EffectiveTo > GETDATE())
	ORDER BY lv.Title
	OFFSET @Skip ROWS 
	FETCH NEXT @Take ROWS ONLY
END
GO
