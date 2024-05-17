/*
	Update User Status to Live for users with Pending Status and GovUkIdentifier is not null or empty
*/

UPDATE dbo.Contacts
SET 
    [Status] = 'Live' 
WHERE 
    [Status] = 'Pending' 
    AND [GovUkIdentifier] IS NOT NULL 
    AND [GovUkIdentifier] != '';

