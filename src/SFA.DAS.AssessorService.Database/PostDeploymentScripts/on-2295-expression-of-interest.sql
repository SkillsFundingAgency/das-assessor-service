/*
	This script will insert an additional report and re-order the report ordering to accomodate the insert
*/

BEGIN TRANSACTION

-- correct previous scripts which have created additional versions of the report on each deployment on test systems
IF (SELECT COUNT(*) FROM [StaffReports] WHERE [StoredProcedure] = N'StaffReports_ExpressionsOfInterest') > 1
BEGIN
	DELETE FROM [StaffReports] WHERE [StoredProcedure] = N'StaffReports_ExpressionsOfInterest'
END

-- insert the new report if it does not already exist
IF NOT EXISTS (SELECT * FROM [StaffReports] WHERE [StoredProcedure] = N'StaffReports_ExpressionsOfInterest')
BEGIN
	INSERT INTO [StaffReports] ([ReportName], [StoredProcedure], [CreatedAt], [DisplayOrder], [ReportType])
	VALUES ('Expression of interest entries', 'StaffReports_ExpressionsOfInterest', GETDATE(), 10, 'ViewOnScreen')
END

COMMIT TRANSACTION