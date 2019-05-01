CREATE PROCEDURE [dbo].EPAO_Register_register_organisation
AS

SET NOCOUNT ON;

-- Register - Organisation
select o.EndPointAssessorOrganisationId as EPA_organisation_identifier, 
    EndPointAssessorName as EPA_Organisation,
    ot.Type as Organisation_Type,
    JSON_VALUE(OrganisationData,'$.WebsiteLink') as Website_Link,
    JSON_VALUE(OrganisationData,'$.Address1') as Contact_address1,
    JSON_VALUE(OrganisationData,'$.Address2') as Contact_address2,
    JSON_VALUE(OrganisationData,'$.Address3') as Contact_address3,
    JSON_VALUE(OrganisationData,'$.Address4') as Contact_address4,
    JSON_VALUE(OrganisationData,'$.Postcode') as Contact_postcode,
    EndPointAssessorUkprn as UKPRN,
    JSON_VALUE(OrganisationData,'$.LegalName') as 'Legal Name'
     from organisations o 
     left outer join organisationType ot on o.OrganisationTypeId = ot.Id
     join (select EndPointAssessorOrganisationId 
           from OrganisationStandard
           where Status = 'Live'
           and (effectiveTo is null OR EffectiveTo > GETDATE())           
           group by EndPointAssessorOrganisationId
     ) ab ON ab.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId
     WHERE o.EndPointAssessorOrganisationId<> 'EPA0000' 
     AND o.[Status] = 'Live'
    order by o.EndPointAssessorOrganisationId

