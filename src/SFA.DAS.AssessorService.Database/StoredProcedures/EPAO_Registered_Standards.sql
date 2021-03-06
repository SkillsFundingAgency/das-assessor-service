﻿CREATE PROCEDURE [dbo].[EPAO_Registered_Standards]
	 @EPAOID AS NVARCHAR(12),
	 @Skip int,
	 @Take int
AS
BEGIN
SELECT 
	os.StandardCode as StandardCode,
	sc.Title as 'StandardName',
	JSON_VALUE(sc.StandardData, '$.Level') as [Level],
	sc.ReferenceNumber as ReferenceNumber
FROM OrganisationStandard os 
	INNER join Organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.EndPointAssessorOrganisationId = @EPAOID
	LEFT outer join StandardCollation sc on os.StandardCode = sc.StandardId
	LEFT outer join Contacts c on os.ContactId = c.Id
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
