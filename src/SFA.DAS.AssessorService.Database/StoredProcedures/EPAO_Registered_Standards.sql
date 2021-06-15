CREATE PROCEDURE [dbo].[EPAO_Registered_Standards]
	 @EPAOID AS NVARCHAR(12),
	 @Skip int,
	 @Take int
AS
BEGIN
WITH LatestVersions
AS
(
	SELECT IFateReferenceNumber, MAX([Version]) As MaxVersion
	FROM Standards
	WHERE Status IN ('Approved for delivery')
	GROUP BY IFateReferenceNumber
),
AppliedVerisons AS
(
	SELECT ab1.StandardReference, MAX(ab1.[Version]) As MaxVersion
	FROM(
    SELECT ap1.Id ApplyId, ap1.ApplicationStatus, ap1.OrganisationId, StandardReference, [Version] FROM Apply ap1
    CROSS APPLY OPENJSON(ApplyData, '$.Apply.Versions') WITH(version CHAR(10) '$')
    ) ab1
    JOIN Organisations og1 on og1.id = ab1.OrganisationId
    WHERE ab1.standardreference IS NOT NULL
		AND og1.EndPointAssessorOrganisationId = @EPAOID
    AND ab1.ApplicationStatus NOT IN('Approved', 'Declined')
	GROUP BY ab1.StandardReference
)
SELECT 
	os.StandardCode as StandardCode,
	sc.Title as 'StandardName',
	JSON_VALUE(sc.StandardData, '$.Level') as [Level],
	sc.ReferenceNumber as ReferenceNumber,
	IIF(lv.MaxVersion > osv.MaxVersion AND (av.MaxVersion IS NULL OR lv.MaxVersion > av.MaxVersion), 1, 0) AS NewVersionAvailable
FROM OrganisationStandard os 
	INNER join (
		SELECT OrganisationStandardId, MAX([Version]) AS MaxVersion
		FROM OrganisationStandardVersion 
		WHERE EffectiveTo is null OR EffectiveTo > GETDATE()
		GROUP BY OrganisationStandardId
	) AS osv ON osv.OrganisationStandardId = os.Id
	INNER join Organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.EndPointAssessorOrganisationId = @EPAOID
	LEFT outer join StandardCollation sc on os.StandardCode = sc.StandardId
	LEFT outer join LatestVersions lv on lv.IFateReferenceNumber = ReferenceNumber
	LEFT outer join AppliedVerisons av on av.StandardReference = ReferenceNumber
	WHERE os.Status = 'Live' 
	and (os.EffectiveTo is null OR os.EffectiveTo > GETDATE())
	and (
		JSON_VALUE(StandardData,'$.EffectiveTo') is null OR
		JSON_VALUE(StandardData,'$.EffectiveTo') > GETDATE()
	)
	ORDER BY 'StandardName'
	OFFSET @Skip ROWS 
    FETCH NEXT @Take ROWS ONLY
END
GO
