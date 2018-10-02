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
	INSERT INTO StaffReports (ReportName, StoredProcedure) VALUES ('By Print Batch', 'StaffReports_ByBatch')
	INSERT INTO StaffReports (ReportName, StoredProcedure) VALUES ('By EPAO', 'StaffReports_ByEpao')
	INSERT INTO StaffReports (ReportName, StoredProcedure) VALUES ('By EPAO, Standard and Grade', 'StaffReports_ByEpaoAndStandardAndGrade')
	INSERT INTO StaffReports (ReportName, StoredProcedure) VALUES ('By Provider', 'StaffReports_ByProvider')
	INSERT INTO StaffReports (ReportName, StoredProcedure) VALUES ('By Provider and Grade', 'StaffReports_ByProviderAndGrade')
	INSERT INTO StaffReports (ReportName, StoredProcedure) VALUES ('By Standard', 'StaffReports_ByStandard')
	INSERT INTO StaffReports (ReportName, StoredProcedure) VALUES ('Certificate Count', 'StaffReports_CertificateCount')
	INSERT INTO StaffReports (ReportName, StoredProcedure) VALUES ('Monthly Detailed Extract', 'StaffReports_DetailedExtract')
	INSERT INTO StaffReports (ReportName, StoredProcedure) VALUES ('Monthly Summary', 'StaffReports_MonthlySummary')
	INSERT INTO StaffReports (ReportName, StoredProcedure) VALUES ('Weekly Summary', 'StaffReports_WeeklySummary')
END