/*
	This pre-deployment script will update the [ContactsPrivileges] and [Privileges] tables to sychronize the
	Id columns with the default lookup data for [Privileges], prior to adding a foreign key to enforce the 
	correct data integrity and the post-deployment script for synchronizing all the columns in [Privileges]
	with the default lookup data
*/
-- these Id's are from the PROD system and are now used as the default lookup data Id's
DECLARE @DefaultApplyForStandardId UNIQUEIDENTIFIER = '0bb7b80d-c090-4520-980b-25b0ca9b9d96'
DECLARE @DefaultChangeOrganisationDetailsId UNIQUEIDENTIFIER = 'ae68553e-999f-46f9-9f36-857099007948'
DECLARE @DefaultRecordGradesId UNIQUEIDENTIFIER = '1a2ab8ef-9759-40d0-b48e-8bdd29fe3866'
DECLARE @DefaultViewCompletedAssessmentsId UNIQUEIDENTIFIER = '2213af32-7e36-41ed-9d90-d9fbd1a60d41'
DECLARE @DefaultManageUsersId UNIQUEIDENTIFIER = '55df950b-4b2f-485f-9106-e67d5cce5afd'
DECLARE @DefaultViewPipelineId UNIQUEIDENTIFIER = 'eb2b783d-4509-4c84-bd35-f056b3b9cad9'

IF((SELECT COUNT(*) FROM [Privileges] WHERE Id NOT IN
	(
		@DefaultApplyForStandardId,
		@DefaultChangeOrganisationDetailsId,
		@DefaultRecordGradesId,
		@DefaultViewCompletedAssessmentsId,
		@DefaultManageUsersId,
		@DefaultViewPipelineId
	)) > 0)
BEGIN
	BEGIN TRANSACTION

	DECLARE @CurrentApplyForStandardId UNIQUEIDENTIFIER = (SELECT Id FROM [Privileges] WHERE [Key] = 'ApplyForStandard')
	DECLARE @CurrentChangeOrganisationDetailsId UNIQUEIDENTIFIER = (SELECT Id FROM [Privileges] WHERE [Key] = 'ChangeOrganisationDetails')
	DECLARE @CurrentRecordGradesId UNIQUEIDENTIFIER = (SELECT Id FROM [Privileges] WHERE [Key] = 'RecordGrades')
	DECLARE @CurrentViewCompletedAssessmentsId UNIQUEIDENTIFIER = (SELECT Id FROM [Privileges] WHERE [Key] = 'ViewCompletedAssessments')
	DECLARE @CurrentManageUsersId UNIQUEIDENTIFIER = (SELECT Id FROM [Privileges] WHERE [Key] = 'ManageUsers')
	DECLARE @CurrentViewPipelineId UNIQUEIDENTIFIER = (SELECT Id FROM [Privileges] WHERE [Key] = 'ViewPipeline')
	
	-- remove rows from [ContactPrivileges] which are not actually for a known [Privileges] row, created whilst FK did not exist
	DELETE FROM [ContactsPrivileges] WHERE PrivilegeId IN
	(
		SELECT DISTINCT cp.PrivilegeId from [ContactsPrivileges] cp left join [Privileges] p on cp.PrivilegeId = p.Id
		WHERE p.Id IS NULL
	)

	PRINT 'UPDATING CONTACTS PRIVILEGES'

	-- update each of the [ContactsPrivileges] to match the known [Privileges] in the default lookup data
	UPDATE [ContactsPrivileges] SET PrivilegeId = @DefaultApplyForStandardId WHERE PrivilegeId = @CurrentApplyForStandardId
	UPDATE [ContactsPrivileges] SET PrivilegeId = @DefaultChangeOrganisationDetailsId WHERE PrivilegeId = @CurrentChangeOrganisationDetailsId
	UPDATE [ContactsPrivileges] SET PrivilegeId = @DefaultRecordGradesId WHERE PrivilegeId = @CurrentRecordGradesId
	UPDATE [ContactsPrivileges] SET PrivilegeId = @DefaultViewCompletedAssessmentsId WHERE PrivilegeId = @CurrentViewCompletedAssessmentsId
	UPDATE [ContactsPrivileges] SET PrivilegeId = @DefaultManageUsersId WHERE PrivilegeId = @CurrentManageUsersId
	UPDATE [ContactsPrivileges] SET PrivilegeId = @DefaultViewPipelineId WHERE PrivilegeId = @CurrentViewPipelineId

	PRINT 'UPDATING PRIVILEGES'

	-- update each of the [Privileges] to match the known [Privileges] in the default lookup data
	UPDATE [Privileges] SET Id = @DefaultApplyForStandardId WHERE Id = @CurrentApplyForStandardId
	UPDATE [Privileges] SET Id = @DefaultChangeOrganisationDetailsId WHERE Id = @CurrentChangeOrganisationDetailsId
	UPDATE [Privileges] SET Id = @DefaultRecordGradesId WHERE Id = @CurrentRecordGradesId
	UPDATE [Privileges] SET Id = @DefaultViewCompletedAssessmentsId WHERE Id = @CurrentViewCompletedAssessmentsId
	UPDATE [Privileges] SET Id = @DefaultManageUsersId WHERE Id = @CurrentManageUsersId
	UPDATE [Privileges] SET Id = @DefaultViewPipelineId WHERE Id = @CurrentViewPipelineId

	COMMIT TRANSACTION
END
