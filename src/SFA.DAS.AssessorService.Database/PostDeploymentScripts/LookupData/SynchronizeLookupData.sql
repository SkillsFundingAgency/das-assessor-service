/*
	Insert, Update or Remove all default lookup data

	NOTES:

	Foreach type of <LookupData> there is a seperate <LookuData>Delete.sql script which should be used to manually remove
	<LookupData> dependencies before removing <LookupData>

	1) Manually removed <LookupData> should also be removed from the <LookupData>InsertOrUpdate.sql script to avoid them
	being restored on the next deployment
	
	2) Manually removing <LookupData> on a branch will be reversed when a different branch containing those
	values is deployed but any manually removed dependencies will not be restored.

	3) The code to manually remove <LookupData> will be left behind in the <LookupData>Delete.sql script which must ITSELF be 
	cleaned up manually	however this is preferred to automatically removing dependencies mainly due to the danger of unintentional deletes
	in the production environment.
*/

:r .\PrivilegesDelete.sql
:r .\PrivilegesInsertOrUpdate.sql

:r .\DeliveryAreaDelete.sql
:r .\DeliveryAreaInsertOrUpdate.sql

:r .\FrameworkDelete.sql
:r .\FrameworkInsertOrUpdate.sql

:r .\OrganisationTypeDelete.sql
:r .\OrganisationTypeInsertOrUpdate.sql

:r .\PostCodeRegionDelete.sql
:r .\PostCodeRegionInsertOrUpdate.sql

:r .\StaffReportsDelete.sql
:r .\StaffReportsInsertOrUpdate.sql

:r .\EmailTemplatesDelete.sql
:r .\EmailTemplatesInsertOrUpdate.sql

:r .\PrePopulateProviders.sql

