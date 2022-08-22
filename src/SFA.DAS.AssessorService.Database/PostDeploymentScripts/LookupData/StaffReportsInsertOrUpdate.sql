/*
	Insert or Update each of the [StaffReports] look up default values.

	NOTES:

	1) This script uses a temporary table, insert or update the values in the temporary table to apply changes; removed values will
	not take affect (by design); values which are removed should also be written into the StaffReportsDelete.sql script to remove
	manually any dependencies, but they must also be removed from the temporary table.
*/
BEGIN TRANSACTION

CREATE TABLE #StaffReports(
	[Id] [uniqueidentifier] NOT NULL,
	[ReportName] [nvarchar](max) NOT NULL,
	[StoredProcedure] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[DisplayOrder] [int] NULL,
	[ReportType] [nvarchar](20) NULL,
	[ReportDetails] [nvarchar](max) NULL
) 
GO

INSERT #StaffReports VALUES (N'54b434be-606b-49b2-afef-1f14c85c48b5', N'Provider and grade', N'StaffReports_ByProviderAndGrade', CAST(N'2018-10-24T13:51:17.8500000' AS DateTime2), NULL, NULL, 8, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'440e1959-20f6-44da-9543-32768057c56d', N'Provider', N'StaffReports_ByProvider', CAST(N'2018-10-24T13:51:17.8333333' AS DateTime2), NULL, NULL, 7, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'b9ea4ba0-3093-4717-ae96-4d19d509a44b', N'Certificate count', N'StaffReports_CertificateCount', CAST(N'2018-10-24T13:51:17.8500000' AS DateTime2), NULL, NULL, 1, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'72079235-bc32-45e1-841f-4f32b8c96b3a', N'Standard', N'StaffReports_ByStandard', CAST(N'2018-10-24T13:51:17.8500000' AS DateTime2), NULL, NULL, 9, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'4d07fe96-ae3e-4476-9a53-673e19314af1', N'EPAO Register', N'', CAST(N'2018-11-28T16:18:40.7866667' AS DateTime2), NULL, NULL, 12, N'Download', N'{"Name":"EPAO Register","Worksheets": [
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
  ]}')
INSERT #StaffReports VALUES (N'caa92d6c-86f6-4f42-823c-77ba775fbc16', N'Monthly detailed extract', N'', CAST(N'2018-10-24T13:51:17.7866667' AS DateTime2), NULL, NULL, 11, N'Download', N'{"Name":"Monthly detailed extract","Worksheets": [ { "worksheet":"Monthly detailed extract", "order": 1, "StoredProcedure":"StaffReports_DetailedExtract" } ]}')
INSERT #StaffReports VALUES (N'2a06aaaa-d3bd-41f7-8907-7d781b4d28a9', N'Expression of interest entries', N'StaffReports_ExpressionsOfInterest', CAST(N'2019-10-11T07:31:04.4433333' AS DateTime2), NULL, NULL, 10, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'37770d79-1733-43f9-90d5-88f0609dd7e2', N'Register List of Standards', N'', CAST(N'2018-11-30T09:42:37.5800000' AS DateTime2), NULL, NULL, 14, N'Download', N'{"Name":"Register List Of Standards","Worksheets": [
	  {
	  "worksheet":"Register List of Standards",
	  "order": 1,
	  "StoredProcedure":"EPAO_Register_list_of_standards"
	  }
	]}')
INSERT #StaffReports VALUES (N'6ecd397a-f7d7-4a13-bb4f-8f8a74bbeace', N'EPAO, standard and grade', N'StaffReports_ByEpaoAndStandardAndGrade', CAST(N'2018-10-24T13:51:17.8333333' AS DateTime2), NULL, NULL, 6, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'3e230675-d61d-4ef0-a678-a254f77c58b7', N'Register List of Organisations', N'', CAST(N'2018-11-30T09:42:37.5633333' AS DateTime2), NULL, NULL, 13, N'Download', N'{"Name":"Register List Of Organisations","Worksheets": [
	  {
	  "worksheet":"Register List of Organisations",
	  "order": 1,
	  "StoredProcedure":"EPAO_Register_list_of_organisations"
	  }
	]}')
INSERT #StaffReports VALUES (N'e6d6ea08-bc88-46b8-bc6c-ced69f191360', N'Monthly summary', N'StaffReports_MonthlySummary', CAST(N'2018-10-24T13:51:17.8033333' AS DateTime2), NULL, NULL, 2, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'4ec6aa79-75ac-4dfe-b277-e2fac7096bda', N'Batch', N'StaffReports_ByBatch', CAST(N'2018-10-24T13:51:17.8200000' AS DateTime2), NULL, NULL, 4, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'd8d8aa09-74b1-4dc4-bbc6-f0b6e71a60cc', N'EPAO', N'StaffReports_ByEpao', CAST(N'2018-10-24T13:51:17.8333333' AS DateTime2), NULL, NULL, 5, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'ec0b30aa-1a6b-4de4-bebd-fb9aaf1a3331', N'Weekly summary', N'StaffReports_WeeklySummary', CAST(N'2018-10-24T13:51:17.8033333' AS DateTime2), NULL, NULL, 3, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'db48b407-6160-426a-9a45-693970d47edc', N'Missing certificate data', N'StaffReports_MissingCertificateData', CAST(N'2021-03-31T00:00:00.0000000' AS DateTime2), NULL, NULL, 15, N'ViewOnScreen', NULL)
INSERT #StaffReports VALUES (N'3800291d-6209-4e65-be4d-e8d390997de8', N'Active Apprentices by Standard', N'', CAST(N'2022-08-22 00:00:00.0000000' AS DateTime2), NULL, NULL, 16, N'Download', N'{"Name":"List Of Active Apprentices by Standard","Worksheets": [
	  {
	  "worksheet": "List Of Active Apprentices",
	  "order": 1,
	  "StoredProcedure": "StaffReports_List_Approved_Standards"
	  }
	]}')

MERGE [StaffReports] [Target] USING #StaffReports [Source]
ON ([Source].[Id] = [Target].[Id])
WHEN MATCHED
    THEN UPDATE SET 
        [Target].[ReportName] = [Source].[ReportName],
        [Target].[StoredProcedure] = [Source].[StoredProcedure],
		[Target].[CreatedAt] = [Source].[CreatedAt], -- TODO: the columns CreatedAt, DeletedAt and UpdatedAt should not be present for lookup data
		[Target].[DeletedAt] = [Source].[DeletedAt],
		[Target].[UpdatedAt] = [Source].[UpdatedAt],
		[Target].[DisplayOrder] = [Source].[DisplayOrder],
		[Target].[ReportType] = [Source].[ReportType],
		[Target].[ReportDetails] = [Source].[ReportDetails]

WHEN NOT MATCHED BY TARGET 
    THEN INSERT ([Id], [ReportName], [StoredProcedure], [CreatedAt], [DeletedAt], [UpdatedAt], [DisplayOrder], [ReportType], [ReportDetails] )
         VALUES ([Source].[Id], [Source].[ReportName], [Source].[StoredProcedure], [Source].[CreatedAt], [Source].[DeletedAt], [Source].[UpdatedAt], [Source].[DisplayOrder], [Source].[ReportType], [Source].[ReportDetails]);

COMMIT TRANSACTION
