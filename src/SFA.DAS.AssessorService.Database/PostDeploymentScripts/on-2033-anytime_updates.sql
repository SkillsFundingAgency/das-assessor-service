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

-- add a priviledge to enable users with the priviledge to edit the organisation details
IF NOT EXISTS (SELECT * FROM [Privileges] WHERE [Key] = N'ChangeOrganisationDetails')
BEGIN
	INSERT INTO [Privileges] (Id, UserPrivilege, MustBeAtLeastOneUserAssigned, Description, [Key], Enabled) 
	VALUES (NEWID(), 'Change organisation details', 0, 'This area allows you to change organisation details', 'ChangeOrganisationDetails', 1)
END

-- add email templates for organisation details change notifications
IF NOT EXISTS (SELECT * FROM [EMailTemplates] WHERE [TemplateName] = N'EPAOOrganisationDetailsAmended')
BEGIN
	INSERT INTO [EMailTemplates] (TemplateName, TemplateId, Recipients, CreatedAt, DeletedAt, UpdatedAt, RecipientTemplate) 
	VALUES ('EPAOOrganisationDetailsAmended', 'd05b7fcd-6aca-4726-8d10-fa36b4172578', NULL, GETUTCDATE(), NULL, NULL, NULL)
END

IF NOT EXISTS (SELECT * FROM [EMailTemplates] WHERE [TemplateName] = N'EPAOPrimaryContactAmended')
BEGIN
	INSERT INTO [EMailTemplates] (TemplateName, TemplateId, Recipients, CreatedAt, DeletedAt, UpdatedAt, RecipientTemplate) 
	VALUES ('EPAOPrimaryContactAmended', 'f87cb8e2-d544-4edd-8dd6-fa1aeeba584b', NULL, GETUTCDATE(), NULL, NULL, NULL)
END

COMMIT TRANSACTION

