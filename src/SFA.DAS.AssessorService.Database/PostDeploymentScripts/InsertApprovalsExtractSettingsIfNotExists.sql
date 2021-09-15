/*
	Insert settings for ApprovalsExtract if they don't exist.
*/

IF NOT EXISTS (SELECT * FROM Settings WHERE Name = 'ApprovalsExtract.StartToleranceS') INSERT INTO Settings (Name, Value) VALUES ('ApprovalsExtract.StartToleranceS', '10')
IF NOT EXISTS (SELECT * FROM Settings WHERE Name = 'ApprovalsExtract.BatchSize') INSERT INTO Settings (Name, Value) VALUES ('ApprovalsExtract.BatchSize', '1000')