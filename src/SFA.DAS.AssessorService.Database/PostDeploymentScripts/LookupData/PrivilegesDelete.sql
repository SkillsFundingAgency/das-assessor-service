/*
	Delete [Privileges] look up default values which are no longer required, this requires manual removal of dependencies:
	
	--> [ContactPrivileges]

	e.g. 
	BEGIN TRANSACTION
		DELETE FROM [ContactPrivileges] WHERE PrivilegeId = '<x>'
		DELETE FROM [Privileges] WHERE Id = '<x>'
	COMMIT TRANSACTION

	NOTES: 
	
	1) Manually removed [Privileges] should also be removed from the PrivilegesInsertOrUpdate.sql script to avoid them
	being restored on the next deployment
	
	2) Manually removing [Privileges] on a branch will be reversed when a different branch containg those
	values is deployed but any manually removed dependencies will not be restored.

	3) Manually removing [Privileges] in this manner will leave behind code which must ITSELF be cleaned up manually
	however this is preferred to automatically removing dependencies mainly due to the danger of unintentional deletes
	in the production environment.
*/