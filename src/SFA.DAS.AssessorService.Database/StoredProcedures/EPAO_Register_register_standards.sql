CREATE PROCEDURE [dbo].[EPAO_Register_register_standards]
AS

SET NOCOUNT ON;
select os.EndPointAssessorOrganisationId as EPA_organisation_identifier,
o.EndPointAssessorName as 'EPA_organisation (lookup auto-populated)',
os.StandardCode as Standard_code,
sc.Title as 'StandardName (lookup auto-populated)',
Convert(nvarchar,[EffectiveFrom],23) as Effective_from,  
Convert(nvarchar,[EffectiveTo],23) as Effective_to,    
c.DisplayName as Contact_name,
c.PhoneNumber as Contact_phonenumber,
c.Email as Contact_email,
Convert(nvarchar,os.DateStandardApprovedOnRegister,23) as 'Date standard Approved on the Register',
os.Comments as Comments
 from organisationStandard os 
inner join organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.EndPointAssessorOrganisationId<> 'EPA0000'
left outer join StandardCollation sc on os.StandardCode = sc.StandardId
left outer join contacts c on os.ContactId = c.Id
where os.status = 'Live'
order by os.EndPointAssessorOrganisationId, os.StandardCode

