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
)
SELECT 
	os.StandardCode as StandardCode,
	sc.Title as 'StandardName',
	JSON_VALUE(sc.StandardData, '$.Level') as [Level],
	sc.ReferenceNumber as ReferenceNumber,
	IIF(lv.MaxVersion > osv.MaxVersion, 1, 0) AS NewVersionAvailable
FROM OrganisationStandard os 
	INNER join (
		SELECT OrganisationStandardId, MAX([Version]) AS MaxVersion
		FROM OrganisationStandardVersion 
		WHERE EffectiveTo is null OR EffectiveTo > GETDATE()
		GROUP BY OrganisationStandardId
	) AS osv ON osv.OrganisationStandardId = os.Id
	INNER join Organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.EndPointAssessorOrganisationId = @EPAOID
	LEFT outer join StandardCollation sc on os.StandardCode = sc.StandardId
	LEFT outer join Contacts c on os.ContactId = c.Id
	LEFT outer join LatestVersions lv on lv.IFateReferenceNumber = ReferenceNumber
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
