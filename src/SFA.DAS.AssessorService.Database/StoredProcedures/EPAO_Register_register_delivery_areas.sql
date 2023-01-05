CREATE PROCEDURE [dbo].[EPAO_Register_register_delivery_areas]
AS

SET NOCOUNT ON;


CREATE TABLE #OrganisationStandardDeliveryAreaDetails
(
    OrganisationStandardId int,
    Cntr int,
    DeliveryAreaList VARCHAR (500)
 
)

insert into #OrganisationStandardDeliveryAreaDetails (OrganisationStandardId, Cntr)
select OrganisationStandardId, count(0) from OrganisationStandardDeliveryArea
group by OrganisationStandardId

update #OrganisationStandardDeliveryAreaDetails set DeliveryAreaList = 'All' where Cntr = (SELECT COUNT(*) FROM [dbo].[DeliveryArea] WHERE [Status] = 'Live')

DECLARE @osId int
DECLARE @details varchar(500)
SELECT @osId = 0

while exists(select * from #OrganisationStandardDeliveryAreaDetails where DeliveryAreaList is null AND OrganisationStandardId != @osId)
  BEGIN
   select top 1 @osId = OrganisationStandardId from #OrganisationStandardDeliveryAreaDetails where DeliveryAreaList is null AND OrganisationStandardId != @osId
          SELECT @details = COALESCE(@details + ', ', '') + Area
                FROM DeliveryArea
                WHERE Id in (select DeliveryAreaId from OrganisationStandardDeliveryArea where OrganisationStandardId = @osId )
  update #OrganisationStandardDeliveryAreaDetails set DeliveryAreaList = @details where OrganisationStandardId = @osId
  set @Details = null
  END
;
WITH Standards_CTE as(
select ROW_NUMBER() OVER (PARTITION BY IFateReferenceNumber ORDER BY VersionMajor DESC, VersionMinor DESC) seq, * from Standards)

select os.EndPointAssessorOrganisationId EPA_organisation_identifier,
  o.EndPointAssessorName as 'EPA_organisation (lookup auto-populated)',
  os.StandardCode as Standard_Code,
  scte.Title as 'StandardName (lookup auto-populated)',
  dad.DeliveryAreaList as Delivery_area,
  JSON_Value(os.OrganisationStandardData,'$.DeliveryAreasComments') as Comments
  --,os.EffectiveTo
 from OrganisationStandard os 
inner join Organisations o on o.EndPointAssessorOrganisationId = os.EndPointAssessorOrganisationId  and o.[Status] = 'Live'
left outer join Standards_CTE scte on os.StandardCode = scte.LarsCode AND seq = 1
left outer join #OrganisationStandardDeliveryAreaDetails dad on dad.OrganisationStandardId = os.Id
where DeliveryAreaList is not null
and o.EndPointAssessorOrganisationId <> 'EPA0000'
and os.[Status] = 'Live'
and (os.EffectiveTo is null OR os.EffectiveTo > GETDATE())
order by o.EndPointAssessorOrganisationId, scte.Title

drop table #OrganisationStandardDeliveryAreaDetails