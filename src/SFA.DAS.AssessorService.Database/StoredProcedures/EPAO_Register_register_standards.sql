CREATE PROCEDURE [dbo].[EPAO_Register_register_standards]
AS

SET NOCOUNT ON;

WITH Standards_CTE as(
select ROW_NUMBER() OVER (PARTITION BY Ifatereferencenumber ORDER BY VersionMajor, VersionMinor) seq, * from Standards)

select os.EndPointAssessorOrganisationId as EPA_organisation_identifier,
o.EndPointAssessorName as 'EPA_organisation (lookup auto-populated)',
os.StandardCode as Standard_code,
scte.Title as 'StandardName (lookup auto-populated)',
Convert(nvarchar,scte.[EffectiveFrom],23) as Effective_from,  
Convert(nvarchar,scte.[EffectiveTo],23) as Effective_to,    
c.DisplayName as Contact_name,
c.PhoneNumber as Contact_phonenumber,
c.Email as Contact_email,
Convert(nvarchar,os.DateStandardApprovedOnRegister,23) as 'Date standard Approved on the Register',
os.Comments as Comments
 from OrganisationStandard os 
inner join Organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.EndPointAssessorOrganisationId<> 'EPA0000' and o.Status = 'Live'
left outer join Standards_CTE scte on os.StandardCode = scte.LarsCode
left outer join Contacts c on os.ContactId = c.Id
where os.[Status] = 'Live'
and (os.EffectiveTo is null OR os.EffectiveTo > GETDATE())
order by os.EndPointAssessorOrganisationId, os.StandardCode

