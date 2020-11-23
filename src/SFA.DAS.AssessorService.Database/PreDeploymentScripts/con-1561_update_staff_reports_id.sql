/*
	This pre-deployment script will update the [StaffReports] table to sychronize the
	Id columns with the default lookup data for [StaffReports]
*/
-- these Id's are from the PROD system and are now used as the default lookup data Id's		 
DECLARE @DefaultMonthlySummary UNIQUEIDENTIFIER = 'E6D6EA08-BC88-46B8-BC6C-CED69F191360'
DECLARE @DefaultWeeklySummary UNIQUEIDENTIFIER = 'EC0B30AA-1A6B-4DE4-BEBD-FB9AAF1A3331'
DECLARE @DefaultBatch UNIQUEIDENTIFIER = '4EC6AA79-75AC-4DFE-B277-E2FAC7096BDA'
DECLARE @DefaultEPAO UNIQUEIDENTIFIER = 'D8D8AA09-74B1-4DC4-BBC6-F0B6E71A60CC'
DECLARE @DefaultEPAOStandardAndGrade UNIQUEIDENTIFIER = '6ECD397A-F7D7-4A13-BB4F-8F8A74BBEACE'
DECLARE @DefaultProvider UNIQUEIDENTIFIER = '440E1959-20F6-44DA-9543-32768057C56D'
DECLARE @DefaultProviderAndGrade UNIQUEIDENTIFIER = '54B434BE-606B-49B2-AFEF-1F14C85C48B5'
DECLARE @DefaultStandard UNIQUEIDENTIFIER = '72079235-BC32-45E1-841F-4F32B8C96B3A'
DECLARE @DefaultExpressionOfInterestEntries UNIQUEIDENTIFIER = '2A06AAAA-D3BD-41F7-8907-7D781B4D28A9'
DECLARE @DefaultMonthlyDetailedExtract UNIQUEIDENTIFIER = 'CAA92D6C-86F6-4F42-823C-77BA775FBC16'
DECLARE @DefaultEPAORegister UNIQUEIDENTIFIER = '4D07FE96-AE3E-4476-9A53-673E19314AF1'
DECLARE @DefaultRegisterListOfOrganisations UNIQUEIDENTIFIER = '3E230675-D61D-4EF0-A678-A254F77C58B7'
DECLARE @DefaultRegisterListOfStandards UNIQUEIDENTIFIER = '37770D79-1733-43F9-90D5-88F0609DD7E2'

IF((SELECT COUNT(*) FROM [StaffReports] WHERE Id NOT IN
	(
		@DefaultMonthlySummary,
		@DefaultWeeklySummary,
		@DefaultBatch,
		@DefaultEPAO,
		@DefaultEPAOStandardAndGrade,
		@DefaultProvider,
		@DefaultProviderAndGrade,
		@DefaultStandard,
		@DefaultExpressionOfInterestEntries,
		@DefaultMonthlyDetailedExtract,
		@DefaultEPAORegister,
		@DefaultRegisterListOfOrganisations,
		@DefaultRegisterListOfStandards
	)) > 0)
BEGIN
	BEGIN TRANSACTION

	DECLARE @CurrentMonthlySummary UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'Monthly summary')
	DECLARE @CurrentWeeklySummary UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'Weekly summary')
	DECLARE @CurrentBatch UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'Batch')
	DECLARE @CurrentEPAO UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'EPAO')
	DECLARE @CurrentEPAOStandardAndGrade UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'EPAO, standard and grade')
	DECLARE @CurrentProvider UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'Provider')
	DECLARE @CurrentProviderAndGrade UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'Provider and grade')
	DECLARE @CurrentStandard UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'Standard')
	DECLARE @CurrentExpressionOfInterestEntries UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'Expression of interest entries')
	DECLARE @CurrentMonthlyDetailedExtract UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'Monthly detailed extract')
	DECLARE @CurrentEPAORegister UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'EPAO Register')
	DECLARE @CurrentRegisterListOfOrganisations UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'Register List of Organisations')
	DECLARE @CurrentRegisterListOfStandards UNIQUEIDENTIFIER = (SELECT Id FROM [StaffReports] WHERE [ReportName] = 'Register List of Standards')

	-- update each of the [StaffReports] to match the known [StaffReports] in the default lookup data
	UPDATE [StaffReports] SET Id = @DefaultMonthlySummary WHERE Id = @CurrentMonthlySummary
	UPDATE [StaffReports] SET Id = @DefaultWeeklySummary WHERE Id = @CurrentWeeklySummary
	UPDATE [StaffReports] SET Id = @DefaultBatch WHERE Id = @CurrentBatch
	UPDATE [StaffReports] SET Id = @DefaultEPAO WHERE Id = @CurrentEPAO
	UPDATE [StaffReports] SET Id = @DefaultEPAOStandardAndGrade WHERE Id = @CurrentEPAOStandardAndGrade
	UPDATE [StaffReports] SET Id = @DefaultProvider WHERE Id = @CurrentProvider
	UPDATE [StaffReports] SET Id = @DefaultProviderAndGrade WHERE Id = @CurrentProviderAndGrade
	UPDATE [StaffReports] SET Id = @DefaultStandard WHERE Id = @CurrentStandard
	UPDATE [StaffReports] SET Id = @DefaultExpressionOfInterestEntries WHERE Id = @CurrentExpressionOfInterestEntries
	UPDATE [StaffReports] SET Id = @DefaultMonthlyDetailedExtract WHERE Id = @CurrentMonthlyDetailedExtract
	UPDATE [StaffReports] SET Id = @DefaultEPAORegister WHERE Id = @CurrentEPAORegister
	UPDATE [StaffReports] SET Id = @DefaultRegisterListOfOrganisations WHERE Id = @CurrentRegisterListOfOrganisations
	UPDATE [StaffReports] SET Id = @DefaultRegisterListOfStandards WHERE Id = @CurrentRegisterListOfStandards

	COMMIT TRANSACTION
END

