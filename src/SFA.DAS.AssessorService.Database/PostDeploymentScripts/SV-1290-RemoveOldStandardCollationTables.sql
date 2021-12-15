/*
	Remove old Standard Collation Tables
*/

IF EXISTS (SELECT TOP(1) * FROM sys.tables WHERE NAME = 'Options') DROP TABLE Options
IF EXISTS (SELECT TOP(1) * FROM sys.tables WHERE NAME = 'StandardCollation') DROP TABLE StandardCollation
IF EXISTS (SELECT TOP(1) * FROM sys.tables WHERE NAME = 'StandardNonApprovedCollation') DROP TABLE StandardNonApprovedCollation

