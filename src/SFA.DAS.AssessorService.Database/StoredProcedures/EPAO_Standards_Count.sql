CREATE PROCEDURE [dbo].[EPAO_Standards_Count] 
@EPAO NVARCHAR(12),
@Count int out
AS

BEGIN

select @Count=Count(*)
from OrganisationStandard os 
inner join Organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.EndPointAssessorOrganisationId = @EPAO
left outer join StandardCollation sc on os.StandardCode = sc.StandardId
left outer join Contacts c on os.ContactId = c.Id
where os.Status = 'Live'
select @Count 
END
GO