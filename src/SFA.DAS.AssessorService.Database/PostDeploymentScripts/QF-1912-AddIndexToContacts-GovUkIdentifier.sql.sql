/*
	Add an index to Contacts.GovUkIdentifier column
*/
CREATE INDEX [IX_Contacts_GovUkIdentifier] ON [Contacts] (GovUkIdentifier)