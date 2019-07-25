/*
	This script will transfer the Email and PhoneNumber of the current organisation Primary Contact (if there is one) to
	addtional properties of the OrganisationData for the organisation.
*/
BEGIN TRANSACTION

/* find the current primary contact for each organisation */
DECLARE @PrimaryOrFirstContact TABLE
(
    ContactId uniqueidentifier null,
    OrganisationId nvarchar(20)
) 

/* using the primary contact email address; if the primary contact does not exist or has been deleted then use the next Live contact for the organisation */
INSERT INTO @PrimaryOrFirstContact (OrganisationId, ContactId) 
SELECT EndPointAssessorOrganisationId, ContactId 
FROM (
  SELECT c.EndPointAssessorOrganisationId, c.Id ContactId, 
     ROW_NUMBER() OVER (PARTITION BY c.EndPointAssessorOrganisationId ORDER BY (CASE WHEN primarycontact = c.username THEN 1 ELSE 0 END) DESC, c.CreatedAt) RowNumber
     FROM [Organisations] o
   JOIN Contacts c ON c.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId
   WHERE c.Status = 'Live'
) ab1 WHERE RowNumber = 1

/* create/update the additonal OrganisationData property where it does not already exist */
UPDATE Organisations
SET 
	OrganisationData = JSON_MODIFY(OrganisationData, '$.Email', c.Email)
FROM
	Organisations o 
		left join @PrimaryOrFirstContact pofc on o.EndPointAssessorOrganisationId = pofc.OrganisationId 
		left join Contacts c on pofc.ContactId = c.Id
WHERE
	JSON_VALUE(OrganisationData, '$.Email') IS NULL

/* create/update the additonal OrganisationData property where it does not already exist */
UPDATE Organisations
SET 
	OrganisationData = JSON_MODIFY(OrganisationData, '$.PhoneNumber', c.PhoneNumber)
FROM
	Organisations o 
		left join @PrimaryOrFirstContact pofc on o.EndPointAssessorOrganisationId = pofc.OrganisationId 
		left join Contacts c on pofc.ContactId = c.Id
WHERE
	JSON_VALUE(OrganisationData, '$.PhoneNumber') IS NULL

COMMIT TRANSACTION