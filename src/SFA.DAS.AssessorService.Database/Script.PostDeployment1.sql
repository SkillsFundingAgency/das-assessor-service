/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
IF NOT EXISTS(SELECT * FROM ScheduleRuns)
BEGIN
	INSERT INTO ScheduleRuns (RunTime, IsComplete, Interval, IsRecurring, ScheduleType) VALUES ('2018-08-17 08:00:00', 0, 10080, 1, 1)
END

IF NOT EXISTS(SELECT * FROM StaffReports)
BEGIN
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Monthly detailed extract', 'StaffReports_DetailedExtract', 1)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Monthly summary', 'StaffReports_MonthlySummary', 2)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Weekly summary', 'StaffReports_WeeklySummary', 3)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Batch', 'StaffReports_ByBatch', 4)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('EPAO', 'StaffReports_ByEpao', 5)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('EPAO, standard and grade', 'StaffReports_ByEpaoAndStandardAndGrade', 6)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Provider', 'StaffReports_ByProvider', 7)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Provider and grade', 'StaffReports_ByProviderAndGrade', 8)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Standard', 'StaffReports_ByStandard', 9)
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder) VALUES ('Certificate count', 'StaffReports_CertificateCount', 10)
END