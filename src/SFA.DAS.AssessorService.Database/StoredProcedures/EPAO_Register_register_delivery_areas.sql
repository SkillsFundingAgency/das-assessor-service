CREATE PROCEDURE [dbo].[EPAO_Register_register_delivery_areas]
AS

SET NOCOUNT ON;


CREATE TABLE #OrganisationStandardDeliveryAreaDetails
(
	organisationStandardId int,
	cntr int,
	DeliveryAreaList VARCHAR (500)
 
)

insert into #OrganisationStandardDeliveryAreaDetails (organisationStandardId, cntr)
select OrganisationStandardId, count(0) from OrganisationStandardDeliveryArea
group by OrganisationStandardId

update #OrganisationStandardDeliveryAreaDetails set DeliveryAreaList = 'All' where cntr = 9

DECLARE @osId int
DECLARE @details varchar(500)
SELECT @osId = 0

while exists(select * from #OrganisationStandardDeliveryAreaDetails where DeliveryAreaList is null AND OrganisationStandardId != @osId)
  BEGIN
   select top 1 @osId = OrganisationStandardId from #OrganisationStandardDeliveryAreaDetails where DeliveryAreaList is null AND OrganisationStandardId != @osId
		  SELECT @details = COALESCE(@details + ', ', '') + Area
				FROM DeliveryArea
				WHERE Id in (select DeliveryAreaId from OrganisationStandardDeliveryArea where OrganisationStandardId = @osId )
  update #OrganisationStandardDeliveryAreaDetails set DeliveryAreaList = @details where organisationStandardId = @osId
  set @Details = null
  END

select os.EndPointAssessorOrganisationId EPA_organisation_identifier,
  o.EndPointAssessorName as 'EPA_organisation (lookup auto-populated)',
  os.StandardCode as Standard_Code,
  sc.Title as 'StandardName (lookup auto-populated)',
  dad.DeliveryAreaList as Delivery_area,
  JSON_Value(os.OrganisationStandardData,'$.DeliveryAreasComments') as Comments
  --,os.EffectiveTo
 from OrganisationStandard os 
inner join Organisations o on o.EndPointAssessorOrganisationId = os.EndPointAssessorOrganisationId --and os.EffectiveTo is null
left outer join StandardCollation sc on os.StandardCode = sc.StandardId
left outer join #OrganisationStandardDeliveryAreaDetails  dad on dad.organisationStandardId = os.Id
where DeliveryAreaList is not null
and o.EndPointAssessorOrganisationId <> 'EPA0000'
order by o.EndPointAssessorOrganisationId, sc.Title

drop table #OrganisationStandardDeliveryAreaDetails

