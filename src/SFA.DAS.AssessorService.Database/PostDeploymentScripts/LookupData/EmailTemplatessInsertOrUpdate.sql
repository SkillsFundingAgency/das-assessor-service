/*
	Insert or Update each of the [EmailTemplates] look up default values.

	NOTES:

	1) This script uses a temporary table, insert or update the values in the temporary table to apply changes; removed values will
	not take affect (by design); values which are removed should also be written into the EmailTemplatesDelete.sql script to remove
	manually any dependencies, but they must also be removed from the temporary table.
*/
BEGIN TRANSACTION


COMMIT TRANSACTION

