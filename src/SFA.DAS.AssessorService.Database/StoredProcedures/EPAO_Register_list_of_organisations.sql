CREATE PROCEDURE [dbo].[EPAO_Register_list_of_organisations]
AS

SET NOCOUNT ON;
-- OPERATION 1: GATHER A LIST OF OrganisationId-ContactId with a one to one, giving PrimaryContact where it is present
CREATE TABLE #PrimaryOrFirstContact
(
	ContactId uniqueidentifier null,
	OrganisationId nvarchar(20)
) 


insert into #PrimaryOrFirstContact (OrganisationId, ContactId) select o.EndPointAssessorOrganisationId, c.Id from organisations o left outer join contacts c on o.PrimaryContact = c.username
 where o.deletedat is null and c.DeletedAt is null
 and o.EndPointAssessorOrganisationId in (select EndPointAssessorOrganisationId from contacts)
 and o.Status !='New'


DECLARE @orgId nvarchar(20)
DECLARE @contactId uniqueidentifier

while exists(select * from #PrimaryOrFirstContact where contactId is null and OrganisationId in (select EndPointAssessorOrganisationId from contacts where deletedat is null))
  BEGIN
   select top 1 @orgId = OrganisationId from #PrimaryOrFirstContact where contactId is null and OrganisationId in (select EndPointAssessorOrganisationId from contacts where deletedat is null)
   SELECT top 1 @contactId = id from contacts where EndPointAssessorOrganisationId = @orgId order by createdat	
  update #PrimaryOrFirstContact set ContactId = @contactId where OrganisationId = @orgId
  set @ContactId = null
  END

-- OPERATION 1 COMPLETED

-- OPERATION 2: CREATE A LIST OF Organisation with up to 4 Delivery Areas, setting area 1 to 'All England' if there are 9 mappings
CREATE TABLE #DeliveryAreaFirstDetails
(
	--id int identity(1,1),
	OrganisationId nvarchar(20),
	DeliveryAreaId int,
	Area nvarchar(256)
) 

insert into  #DeliveryAreaFirstDetails
select distinct os.EndPointAssessorOrganisationId, osda.DeliveryAreaId, da.Area 
	from OrganisationStandardDeliveryArea osda 
	inner join OrganisationStandard os on osda.OrganisationStandardId = os.Id
	inner join DeliveryArea da on osda.DeliveryAreaId = da.Id
	order by EndPointAssessorOrganisationId, DeliveryAreaId

CREATE TABLE #DeliveryAreaSummary
(
	OrganisationId nvarchar(20),
	Delivery_Area_1 nvarchar(256),
	Delivery_Area_2 nvarchar(256),
	Delivery_Area_3 nvarchar(256),
	Delivery_Area_4 nvarchar(256),
)

-- OPERATION 2.1 set organisations mapped to all DeliveryAreas to 'All England'
insert into #DeliveryAreaSummary (OrganisationId, Delivery_Area_1) 
	select OrganisationId, 'All England' from #DeliveryAreaFirstDetails 
		group by organisationId
		having count(0)>=(select count(0) from DeliveryArea)

-- OPERATION 2.2 set organisations mapped to one and only one delivery area to to have a fixed Delivery_area_1
insert into #DeliveryAreaSummary (OrganisationId, Delivery_Area_1)
select OrganisationId, Max(Area) area from #DeliveryAreaFirstDetails 
		group by organisationId
		having count(0)=1

delete from #DeliveryAreaFirstDetails where organisationId in (select OrganisationId from #DeliveryAreaSummary)

insert into #DeliveryAreaSummary (OrganisationId)
	select distinct OrganisationId from #DeliveryAreaFirstDetails

select organisationId, Area,
    row_number() over(partition by organisationId order by organisationId) seq into #sequencedAreaList
  from #DeliveryAreaFirstDetails
  order by OrganisationId, seq

update das  set Delivery_Area_1 = sal.Area 
	from #DeliveryAreaSummary das
	left join #sequencedAreaList sal on sal.OrganisationId = das.OrganisationId and sal.seq =1
	where Delivery_Area_1 is null

update das  set Delivery_Area_2 = sal.Area
	from #DeliveryAreaSummary das
	left join #sequencedAreaList sal on sal.OrganisationId = das.OrganisationId and sal.seq =2

update das  set Delivery_Area_2 = sal.Area
	from #DeliveryAreaSummary das
	left join #sequencedAreaList sal on sal.OrganisationId = das.OrganisationId and sal.seq =2
	
update das  set Delivery_Area_3 = sal.Area
	from #DeliveryAreaSummary das
	left join #sequencedAreaList sal on sal.OrganisationId = das.OrganisationId and sal.seq =3
	
update das  set Delivery_Area_4 =  sal.Area
	from #DeliveryAreaSummary das
	left join #sequencedAreaList sal on sal.OrganisationId = das.OrganisationId and sal.seq =4

update das  set Delivery_Area_4 = Delivery_Area_4 + '; ' +sal.Area
	from #DeliveryAreaSummary das
	left join #sequencedAreaList sal on sal.OrganisationId = das.OrganisationId and sal.seq =5
	where sal.Area is not null

update das  set Delivery_Area_4 = Delivery_Area_4 + '; ' +sal.Area
	from #DeliveryAreaSummary das
	left join #sequencedAreaList sal on sal.OrganisationId = das.OrganisationId and sal.seq =6
	where sal.Area is not null

update das  set Delivery_Area_4 = Delivery_Area_4 + '; ' +sal.Area
	from #DeliveryAreaSummary das
	left join #sequencedAreaList sal on sal.OrganisationId = das.OrganisationId and sal.seq =7
	where sal.Area is not null

update das  set Delivery_Area_4 = Delivery_Area_4 + '; ' +sal.Area
	from #DeliveryAreaSummary das
	left join #sequencedAreaList sal on sal.OrganisationId = das.OrganisationId and sal.seq =8
	where sal.Area is not null

update das  set Delivery_Area_4 = Delivery_Area_4 + '; ' +sal.Area
	from #DeliveryAreaSummary das
	left join #sequencedAreaList sal on sal.OrganisationId = das.OrganisationId and sal.seq =9
	where sal.Area is not null

drop table #sequencedAreaList

-- OPERATION 2 COMPLETED

-- OPERATION 3 Gather and pivot Standard title and level
-- GAther standard details, excluding those that have expired or expire today
select os.EndPointAssessorOrganisationId as organisationid, Title + ' - Level ' + JSON_Value(StandardData,'$.Level') as StandardDetails 
	into #StandardDetails
	from organisationStandard os 
	inner join standardCollation sc on os.StandardCode = sc.StandardId 
	where StandardData is not NULL
	and (os.effectiveTo is null OR os.EffectiveTo > GETDATE())
	and (
		JSON_Value(StandardData,'$.EffectiveTo') is null OR
		JSON_Value(StandardData,'$.EffectiveTo') > GETDATE()
		)
	order by EndPointAssessorOrganisationId, sc.Title

select organisationId, StandardDetails,
    row_number() over(partition by organisationId order by organisationId) seq into #sequencedStandardDetails
  from #StandardDetails
  order by OrganisationId, seq

CREATE TABLE #OrganisationStandardTableSummary
(
	OrganisationId nvarchar(20),
	Standard_1 nvarchar(500),
	Standard_2 nvarchar(500),
	Standard_3 nvarchar(500),
	Standard_4 nvarchar(500),
	Standard_5 nvarchar(500),
	Standard_6 nvarchar(500),
	Standard_7 nvarchar(500),
	Standard_8 nvarchar(500),
	Standard_9 nvarchar(500),	
	Standard_10 nvarchar(500),
	Standard_11 nvarchar(500),
	Standard_12 nvarchar(500),
	Standard_13 nvarchar(500),
	Standard_14 nvarchar(500),
	Standard_15 nvarchar(500),
	Standard_16 nvarchar(500),
	Standard_17 nvarchar(500),
	Standard_18 nvarchar(500),
	Standard_19 nvarchar(500),	
	Standard_20 nvarchar(500),
	Standard_21 nvarchar(500),
	Standard_22 nvarchar(500),
	Standard_23 nvarchar(500),
	Standard_24 nvarchar(500),
	Standard_25 nvarchar(500),
	Standard_26 nvarchar(500),
	Standard_27 nvarchar(500),
	Standard_28 nvarchar(500),
	Standard_29 nvarchar(500),	
	Standard_30 nvarchar(500),
	Standard_31 nvarchar(500),
	Standard_32 nvarchar(500),
	Standard_33 nvarchar(500),
	Standard_34 nvarchar(500),
	Standard_35 nvarchar(500),
	Standard_36 nvarchar(500),
	Standard_37 nvarchar(500),
	Standard_38 nvarchar(500),
	Standard_39 nvarchar(500),	
	Standard_40 nvarchar(500),
	Standard_41 nvarchar(500),
	Standard_42 nvarchar(500),
	Standard_43 nvarchar(500),
	Standard_44 nvarchar(500),
	Standard_45 nvarchar(500),
	Standard_46 nvarchar(500),
	Standard_47 nvarchar(500),
	Standard_48 nvarchar(500),
	Standard_49 nvarchar(500),	
	Standard_50 nvarchar(500),
	Standard_51 nvarchar(500),
	Standard_52 nvarchar(500),
	Standard_53 nvarchar(500),
	Standard_54 nvarchar(500),
	Standard_55 nvarchar(500),
	Standard_56 nvarchar(500),
	Standard_57 nvarchar(500),
	Standard_58 nvarchar(500),
	Standard_59 nvarchar(500),	
	Standard_60 nvarchar(500),
)

-- OPERATION 3.2 Create containment table for details and populate table OrganisationStandardTableSummary
insert into #OrganisationStandardTableSummary (OrganisationId) select distinct OrganisationId from #sequencedStandardDetails

DECLARE @SQLToUpdateStandardDetails varchar(max)

DECLARE @cnt INT = 1;

WHILE @cnt < 61
BEGIN
  select @SQLToUpdateStandardDetails ='update osts  set Standard_' + convert(varchar,@cnt) + ' = sas.StandardDetails from #OrganisationStandardTableSummary osts left join #sequencedStandardDetails sas on sas.OrganisationId = osts.OrganisationId and sas.seq =' + convert(varchar,@cnt);
	exec(@SQLToUpdateStandardDetails)
   SET @cnt = @cnt + 1;
END;

drop table #StandardDetails
drop table #sequencedStandardDetails

select o.EndPointAssessorName as EP_Assessment_Organisations, 
o.EndPointAssessorOrganisationId as EPA_ORG_ID,
c1.DisplayName as Contact_Name,
JSON_VALUE(OrganisationData,'$.Address1') as Contact_address1,
JSON_VALUE(OrganisationData,'$.Address2') as Contact_address2,
JSON_VALUE(OrganisationData,'$.Address3') as Contact_address3,
JSON_VALUE(OrganisationData,'$.Address4') as Contact_address4,
JSON_VALUE(OrganisationData,'$.Postcode') as Postcode,
C1.PhoneNumber as Contact_number,
C1.Email as Contact_email,
das.Delivery_Area_1,
das.Delivery_Area_2,
das.Delivery_Area_3,
das.Delivery_Area_4,
ot.Type as Organisation_type,
JSON_VALUE(OrganisationData, '$.WebsiteLink') as Link_to_website,
Standard_1,
Standard_2,
	Standard_3,
	Standard_4,
	Standard_5,
	Standard_6,
	Standard_7,
	Standard_8,
	Standard_9,	
	Standard_10,
	Standard_11,
	Standard_12,
	Standard_13,
	Standard_14,
	Standard_15,
	Standard_16,
	Standard_17,
	Standard_18,
	Standard_19,	
	Standard_20,
	Standard_21,
	Standard_22,
	Standard_23,
	Standard_24,
	Standard_25,
	Standard_26,
	Standard_27,
	Standard_28,
	Standard_29,	
	Standard_30,
	Standard_31,
	Standard_32,
	Standard_33,
	Standard_34,
	Standard_35,
	Standard_36,
	Standard_37,
	Standard_38,
	Standard_39,	
	Standard_40,
	Standard_41,
	Standard_42,
	Standard_43,
	Standard_44,
	Standard_45,
	Standard_46,
	Standard_47,
	Standard_48,
	Standard_49,	
	Standard_50,
	Standard_51,
	Standard_52,
	Standard_53,
	Standard_54,
	Standard_55,
	Standard_56,
	Standard_57,
	Standard_58,
	Standard_59,	
	Standard_60
 from organisations o 
left outer join #PrimaryOrFirstContact pofc on pofc.OrganisationId = o.EndPointAssessorOrganisationId
left outer join contacts c1 on c1.EndPointAssessorOrganisationId = pofc.OrganisationId and c1.Id = pofc.ContactId
left outer join #DeliveryAreaSummary das on o.EndPointAssessorOrganisationId = das.OrganisationId
left outer join OrganisationType ot on o.OrganisationTypeId = ot.Id 
left outer join #OrganisationStandardTableSummary osts on osts.OrganisationId = o.EndPointAssessorOrganisationId
where o.deletedat is NULL AND o.EndPointAssessorOrganisationId<>'EPA0000'
and c1.DeletedAt is null
order by o.EndPointAssessorName

drop table #DeliveryAreaSummary
drop table #DeliveryAreaFirstDetails
drop table #PrimaryOrFirstContact
drop table #OrganisationStandardTableSummary