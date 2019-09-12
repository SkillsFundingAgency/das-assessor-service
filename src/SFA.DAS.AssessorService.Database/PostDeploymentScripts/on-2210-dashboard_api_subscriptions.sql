/*
	This script will remove the unused 'ManageAPISubscription' privilege and transfers it's purpose
	to the 'ChangeOrganisationDetails' privilege; the description will be transformed into privilege
	data with multiple rights so that each purpose can be listed
*/
BEGIN TRANSACTION

IF EXISTS (SELECT * FROM [Privileges] WHERE [Key] = N'ManageAPISubscription')
BEGIN
	DELETE FROM [Privileges] WHERE [Key] = 'ManageAPISubscription'
END

IF EXISTS (SELECT * FROM [Privileges] WHERE [Key] = N'ChangeOrganisationDetails')
BEGIN
	UPDATE Privileges SET PrivilegeData = '{"Rights":["change contact details on the Register", "manage API key"]}' WHERE [Key] = 'ChangeOrganisationDetails'	
END

IF EXISTS (SELECT * FROM [Privileges] WHERE [Key] = N'ViewPipeline')
BEGIN
	UPDATE Privileges SET PrivilegeData = '{"Rights":["view the Standard and number of apprentices due to be assessed"]}' WHERE [Key] = 'ViewPipeline'
END

IF EXISTS (SELECT * FROM [Privileges] WHERE [Key] = N'ViewCompletedAssessments')
BEGIN
	UPDATE Privileges SET PrivilegeData = '{"Rights":["view all previously recorded assessments"]}' WHERE [Key] = 'ViewCompletedAssessments'
END

IF EXISTS (SELECT * FROM [Privileges] WHERE [Key] = N'ApplyForStandard')
BEGIN
	UPDATE Privileges SET PrivilegeData = '{"Rights":["apply for a Standard"]}' WHERE [Key] = 'ApplyForStandard'
END

IF EXISTS (SELECT * FROM [Privileges] WHERE [Key] = N'ManageUsers')
BEGIN
	UPDATE Privileges SET PrivilegeData = '{"Rights":["view a list of all users in your organisation", "manage user permissions"]}' WHERE [Key] = 'ManageUsers'
END

IF EXISTS (SELECT * FROM [Privileges] WHERE [Key] = N'RecordGrades')
BEGIN
	UPDATE Privileges SET PrivilegeData = '{"Rights":["record assessment grades and produce certificates"]}' WHERE [Key] = 'RecordGrades'
END

COMMIT TRANSACTION

