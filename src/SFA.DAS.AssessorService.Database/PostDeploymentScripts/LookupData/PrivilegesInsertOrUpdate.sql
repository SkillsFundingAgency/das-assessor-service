/*
	Insert or Update each of the [Privileges] look up default values.

	NOTES:

	1) This script uses a temporary table, insert or update the values in the temporary table to apply changes; removed values will
	not take affect (by design); values which are removed should also be written into the PrivilegesDelete.sql script to remove
	manually any dependencies, but they must also be removed from the temporary table.

	2) The [Privileges] table has an [Enabled] column which can be used to prevent the privilege allowing access to a feature; this
	may be preferable to actually deleting the [Privilege]; however the privilege would still be assignable to users in this case.
*/
BEGIN TRANSACTION

CREATE TABLE #Privileges(
	[Id] [uniqueidentifier] NOT NULL,
	[UserPrivilege] [nvarchar](120) NOT NULL,
	[MustBeAtLeastOneUserAssigned] [bit] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[PrivilegeData] [nvarchar](max) NOT NULL,
	[Key] [nvarchar](125) NOT NULL,
	[Enabled] [bit] NOT NULL,
) 

INSERT #Privileges VALUES (N'0bb7b80d-c090-4520-980b-25b0ca9b9d96', N'Apply for a Standard', 0, N'This area allows you to apply for a Standard.', N'{"Rights":["apply for a Standard"]}', N'ApplyForStandard', 1)
INSERT #Privileges VALUES (N'ae68553e-999f-46f9-9f36-857099007948', N'Change organisation details', 0, N'This area allows you to change organisation details', N'{"Rights":["change contact details on the Register", "manage API key"]}', N'ChangeOrganisationDetails', 1)
INSERT #Privileges VALUES (N'1a2ab8ef-9759-40d0-b48e-8bdd29fe3866', N'Record grades and issue certificates', 0, N'This area allows you to record assessment grades and produce certificates.', N'{"Rights":["record assessment grades and produce certificates"]}', N'RecordGrades', 1)
INSERT #Privileges VALUES (N'2213af32-7e36-41ed-9d90-d9fbd1a60d41', N'Completed assessments', 0, N'This area shows all previously recorded assessments.', N'{"Rights":["view all previously recorded assessments"]}', N'ViewCompletedAssessments', 1)
INSERT #Privileges VALUES (N'55df950b-4b2f-485f-9106-e67d5cce5afd', N'Manage users', 1, N'This area shows a list of all users in your organisation and the ability to manage their permissions.', N'{"Rights":["view a list of all users in your organisation", "manage user permissions"]}', N'ManageUsers', 1)
INSERT #Privileges VALUES (N'eb2b783d-4509-4c84-bd35-f056b3b9cad9', N'Pipeline', 0, N'This area shows the Standard and number of apprentices due to be assessed.', N'{"Rights":["view the Standard and number of apprentices due to be assessed"]}', N'ViewPipeline', 1)

MERGE [Privileges] [Target] USING #Privileges [Source]
ON ([Source].Id = [Target].Id)
WHEN MATCHED
    THEN UPDATE SET 
        [Target].[UserPrivilege] = [Source].[UserPrivilege],
        [Target].[MustBeAtLeastOneUserAssigned] = [Source].[MustBeAtLeastOneUserAssigned],
		[Target].[Description] = [Source].[Description],
		[Target].[PrivilegeData] = [Source].[PrivilegeData],
		[Target].[Key] = [Source].[Key],
		[Target].[Enabled] = [Source].[Enabled]

WHEN NOT MATCHED BY TARGET 
    THEN INSERT ([Id], [UserPrivilege], [MustBeAtLeastOneUserAssigned], [Description], [PrivilegeData], [Key], [Enabled])
         VALUES ([Source].[Id], [Source].[UserPrivilege], [Source].[MustBeAtLeastOneUserAssigned], [Source].[Description], [Source].[PrivilegeData], [Source].[Key], [Source].[Enabled]);

COMMIT TRANSACTION

