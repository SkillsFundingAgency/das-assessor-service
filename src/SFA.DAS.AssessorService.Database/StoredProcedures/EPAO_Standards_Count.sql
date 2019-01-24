CREATE PROCEDURE [dbo].[EPAO_Standards_Count] 
@EPAO NVARCHAR(12),
@Count int out
AS

BEGIN

select @Count=Count(*)
from organisationStandard os 
inner join organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.EndPointAssessorOrganisationId = @EPAO
left outer join StandardCollation sc on os.StandardCode = sc.StandardId
left outer join contacts c on os.ContactId = c.Id
where os.status = 'Live'
select @Count 
END
GO