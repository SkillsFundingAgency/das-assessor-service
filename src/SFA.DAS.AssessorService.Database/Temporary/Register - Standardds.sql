
select os.EndPointAssessorOrganisationId as EPA_organisation_identifier,
o.EndPointAssessorName as 'EPA_organisation (lookup auto-populated)',
os.StandardCode as Standard_code,
sc.Title as 'StandardName (lookup auto-populated)',
JSON_VALUE(StandardData,'$.EffectiveFrom') as Effective_from,
JSON_VALUE(StandardData,'$.EffectiveTo') as Effective_to,
c.DisplayName as Contact_name,
c.PhoneNumber as Contact_phonenumber,
c.Email as Contact_email,
os.DateStandardApprovedOnRegister as 'Date standard Approved on the Register',
os.Comments as Comments
 from organisationStandard os 
inner join organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId
left outer join StandardCollation sc on os.StandardCode = sc.StandardId
left outer join contacts c on os.ContactId = c.Id
order by os.EndPointAssessorOrganisationId, os.StandardCode

