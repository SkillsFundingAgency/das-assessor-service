/*
	This script will update the report names and display order to match the current PROD values, the script should
	NOT be used to add addtional reports use a seperate idempotent script and then update this script to set the
	overall display ordering and report names
*/
BEGIN TRANSACTION
	UPDATE StaffReports SET ReportName = 'Provider and grade' WHERE StoredProcedure = 'StaffReports_ByProviderAndGrade'
	UPDATE StaffReports SET ReportName = 'Provider' WHERE StoredProcedure = 'StaffReports_ByProvider'
	UPDATE StaffReports SET ReportName = 'Certificate count' WHERE StoredProcedure = 'StaffReports_CertificateCount'
	UPDATE StaffReports SET ReportName = 'Standard' WHERE StoredProcedure = 'StaffReports_ByStandard'
	UPDATE StaffReports SET ReportName = 'Monthly detailed extract' WHERE StoredProcedure = 'StaffReports_DetailedExtract'
	UPDATE StaffReports SET ReportName = 'EPAO, standard and grade' WHERE StoredProcedure = 'StaffReports_ByEpaoAndStandardAndGrade'
	UPDATE StaffReports SET ReportName = 'Monthly summary' WHERE StoredProcedure = 'StaffReports_MonthlySummary'
	UPDATE StaffReports SET ReportName = 'Batch' WHERE StoredProcedure = 'StaffReports_ByBatch'
	UPDATE StaffReports SET ReportName = 'EPAO' WHERE StoredProcedure = 'StaffReports_ByEpao'
	UPDATE StaffReports SET ReportName = 'Weekly summary' WHERE StoredProcedure = 'StaffReports_WeeklySummary'
	UPDATE StaffReports SET ReportName = 'Expression of interest entries' WHERE StoredProcedure = 'StaffReports_ExpressionsOfInterest'

	UPDATE StaffReports SET DisplayOrder = 1  WHERE ReportName = 'Certificate count'
	UPDATE StaffReports SET DisplayOrder = 2  WHERE ReportName = 'Monthly summary'
	UPDATE StaffReports SET DisplayOrder = 3  WHERE ReportName = 'Weekly summary'
	UPDATE StaffReports SET DisplayOrder = 4  WHERE ReportName = 'Batch'
	UPDATE StaffReports SET DisplayOrder = 5  WHERE ReportName = 'EPAO'
	UPDATE StaffReports SET DisplayOrder = 6  WHERE ReportName = 'EPAO, standard and grade'
	UPDATE StaffReports SET DisplayOrder = 7  WHERE ReportName = 'Provider'
	UPDATE StaffReports SET DisplayOrder = 8  WHERE ReportName = 'Provider and grade'
	UPDATE StaffReports SET DisplayOrder = 9  WHERE ReportName = 'Standard'
	UPDATE StaffReports SET DisplayOrder = 10 WHERE ReportName = 'Expression of interest entries'
	UPDATE StaffReports SET DisplayOrder = 11 WHERE ReportName = 'Monthly detailed extract'
	UPDATE StaffReports SET DisplayOrder = 12 WHERE ReportName = 'EPAO Register'
	UPDATE StaffReports SET DisplayOrder = 13 WHERE ReportName = 'Register List of Organisations'
	UPDATE StaffReports SET DisplayOrder = 14 WHERE ReportName = 'Register List of Standards'

	-- this changes report on screen to download only.
	UPDATE StaffReports SET StoredProcedure = '', ReportType = 'Download',
	ReportDetails = '{"Name":"Monthly detailed extract","Worksheets": [ { "worksheet":"Monthly detailed extract", "order": 1, "StoredProcedure":"StaffReports_DetailedExtract" } ]}'
	WHERE ReportName = 'Monthly detailed extract' 

COMMIT TRANSACTION




