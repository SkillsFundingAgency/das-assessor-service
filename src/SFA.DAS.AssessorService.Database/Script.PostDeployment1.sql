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
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Monthly detailed extract', 'StaffReports_DetailedExtract', 1,'ViewOnScreen')
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Monthly summary', 'StaffReports_MonthlySummary', 2,'ViewOnScreen')
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Weekly summary', 'StaffReports_WeeklySummary', 3,'ViewOnScreen')
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Batch', 'StaffReports_ByBatch', 4,'ViewOnScreen')
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('EPAO', 'StaffReports_ByEpao', 5,'ViewOnScreen')
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('EPAO, standard and grade', 'StaffReports_ByEpaoAndStandardAndGrade', 6,'ViewOnScreen')
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Provider', 'StaffReports_ByProvider', 7,'ViewOnScreen')
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Provider and grade', 'StaffReports_ByProviderAndGrade', 8,'ViewOnScreen')
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Standard', 'StaffReports_ByStandard', 9,'ViewOnScreen')
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('Certificate count', 'StaffReports_CertificateCount', 10,'ViewOnScreen')
END

UPDATE StaffReports SET ReportType = 'ViewOnScreen' WHERE ReportType IS NULL

IF NOT EXISTS (SELECT * FROM StaffReports WHERE ReportName = 'EPAO Register')
BEGIN
	INSERT INTO StaffReports (ReportName, StoredProcedure, DisplayOrder, ReportType) VALUES ('EPAO Register', '', 11, 'Download')
END


UPDATE StaffReports SET ReportDetails ='{"Name":"EPAO Register","Type":"excel","Worksheets": [
  {
  "worksheet":"Register - Organisations",
  "order": 1,
  "StoredProcedure":"EPAO_Register_register_organisation"
  },
  {
  "worksheet":"Register - Standards",
  "order": 2,
  "StoredProcedure":"EPAO_Register_register_standards"
  },
  {
  "worksheet":"Register - Delivery areas",
  "order": 3,
  "StoredProcedure":"EPAO_Register_register_delivery_areas"
  },
  {
  "worksheet":"Data Definitions",
  "order": 4,
  "StoredProcedure":"EPAO_Register_Data_Definitions"
  }
  ]}' WHERE ReportName = 'EPAO Register'


UPDATE CERTIFICATES
set IsPrivatelyFunded = 0
WHERE IsPrivatelyFunded IS NULL 