-- make monthly detailed extract a download only.

-- this sets the reportnames and displayorder to PROD

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


UPDATE StaffReports SET DisplayOrder = 1  WHERE ReportName = 'Certificate count'
UPDATE StaffReports SET DisplayOrder = 2  WHERE ReportName = 'Monthly summary'
UPDATE StaffReports SET DisplayOrder = 3  WHERE ReportName = 'Weekly summary'
UPDATE StaffReports SET DisplayOrder = 4  WHERE ReportName = 'Batch'
UPDATE StaffReports SET DisplayOrder = 5  WHERE ReportName = 'EPAO'
UPDATE StaffReports SET DisplayOrder = 6  WHERE ReportName = 'EPAO, standard and grade'
UPDATE StaffReports SET DisplayOrder = 7  WHERE ReportName = 'Provider'
UPDATE StaffReports SET DisplayOrder = 8  WHERE ReportName = 'Provider and grade'
UPDATE StaffReports SET DisplayOrder = 9  WHERE ReportName = 'Standard'

-- On-2295
INSERT INTO [StaffReports] ([ReportName],[StoredProcedure],CreatedAt,DisplayOrder,ReportType)
VALUES ('Expression of interest entries','StaffReports_ExpressionsOfInterest',GETDATE(),10,'ViewOnScreen')

UPDATE StaffReports SET DisplayOrder = 11 WHERE ReportName = 'Monthly detailed extract'
UPDATE StaffReports SET DisplayOrder = 12 WHERE ReportName = 'EPAO Register'
UPDATE StaffReports SET DisplayOrder = 13 WHERE ReportName = 'Register List of Organisations'
UPDATE StaffReports SET DisplayOrder = 14 WHERE ReportName = 'Register List of Standards'

-- this changes report on screen to download only.
update StaffReports Set Storedprocedure = '', reporttype = 'Download',
reportdetails = '{"Name":"Monthly detailed extract","Worksheets": [ { "worksheet":"Monthly detailed extract", "order": 1, "StoredProcedure":"StaffReports_DetailedExtract" } ]}'
where reportname = 'Monthly detailed extract' 





