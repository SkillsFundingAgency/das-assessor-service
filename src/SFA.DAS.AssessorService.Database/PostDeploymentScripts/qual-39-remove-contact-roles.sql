/*
	This script will drop the superfluous table ContactRoles as it is no longer required.
*/

BEGIN TRANSACTION

IF OBJECT_ID('dbo.ContactRoles', 'U') IS NOT NULL 
  DROP TABLE [dbo].[ContactRoles]; 

COMMIT TRANSACTION