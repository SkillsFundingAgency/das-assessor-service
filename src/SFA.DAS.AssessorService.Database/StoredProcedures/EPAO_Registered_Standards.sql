CREATE PROCEDURE [dbo].[EPAO_Registered_Standards]
	 @EPAOID AS NVARCHAR(12),
	 @Skip int,
	 @Take int
AS
BEGIN
select 
	os.StandardCode as StandardCode,
	sc.Title as 'StandardName',
	sc.ReferenceNumber as 'StandardReference',
	JSON_VALUE(sc.StandardData, '$.Level') as [Level] FROM organisationStandard os 
	INNER join organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.EndPointAssessorOrganisationId = @EPAOID
	LEFT outer join StandardCollation sc on os.StandardCode = sc.StandardId
	LEFT outer join contacts c on os.ContactId = c.Id
	WHERE os.status = 'Live' ORDER BY 'StandardName'
	OFFSET @Skip ROWS 
    FETCH NEXT @Take ROWS ONLY
END
GO
