/*
	Temporary script to update the Status for Pending users with a non-null GovUkIdentifier to Live.  
    Only needs to be run once on each environment.
*/

UPDATE dbo.Contacts
SET 
    [Status] = 'Live' 
WHERE 
    [Status] = 'Pending' 
    AND [GovUkIdentifier] IS NOT NULL 
    AND [GovUkIdentifier] != '';

