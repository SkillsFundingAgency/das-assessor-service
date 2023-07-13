/*
	Insert or Update each of the [OrganisationTypes] look up default values.

	NOTES:

	1) This script uses a temporary table, insert or update the values in the temporary table to apply changes; removed values will
	not take affect (by design); values which are removed should also be written into the OrganisationTypesDelete.sql script to remove
	manually any dependencies, but they must also be removed from the temporary table.
*/
BEGIN TRANSACTION

CREATE TABLE #OrganisationType(
	[Id] [int] NOT NULL,
	[Type] [nvarchar](256) NOT NULL,
	[Status] [nvarchar](10) NOT NULL,
	[TypeDescription] [nvarchar](500) NULL,
	[FinancialExempt] [bit] NOT NULL,
) 

INSERT #OrganisationType VALUES (1, N'Awarding Organisations', N'Live', N'Awarding organisations', 1)
INSERT #OrganisationType VALUES (2, N'Assessment Organisations', N'Live', N'Assessment organisations', 1)
INSERT #OrganisationType VALUES (3, N'Trade Body', N'Live', N'Trade body', 1)
INSERT #OrganisationType VALUES (4, N'Professional Body', N'Live', N'Professional body - approved by the relevant council', 1)
INSERT #OrganisationType VALUES (5, N'HEI', N'Live', N'Higher education institute (HEI) monitored and supported by the Office for Students (OfS)', 1)
INSERT #OrganisationType VALUES (6, N'NSA or SSC', N'Live', N'National skills academy or sector skills council', 1)
INSERT #OrganisationType VALUES (7, N'Training Provider', N'Live', N'Training provider - including HEI not in England', 1)
INSERT #OrganisationType VALUES (8, N'Other', N'Deleted', NULL, 1)
INSERT #OrganisationType VALUES (9, N'Public Sector', N'Live', N'Incorporated as a public sector body', 1)
INSERT #OrganisationType VALUES (10, N'College', N'Live', N'General further education (GFE) college currently receiving funding from the ESFA, 6th form or further education (FE) college', 1)
INSERT #OrganisationType VALUES (11, N'Academy or Free School', N'Live', N'Academy or Free school registered with the ESFA', 1)

SET IDENTITY_INSERT [dbo].[OrganisationType] ON 

MERGE [OrganisationType] [Target] USING #OrganisationType [Source]
ON ([Source].Id = [Target].Id)
WHEN MATCHED
    THEN UPDATE SET 
        [Target].[Type] = [Source].[Type],
        [Target].[Status] = [Source].[Status],
        [Target].[TypeDescription] = [Source].[TypeDescription],
		[Target].[FinancialExempt] = [Source].[FinancialExempt]

WHEN NOT MATCHED BY TARGET 
    THEN INSERT ([Id], [Type], [Status], [TypeDescription], [FinancialExempt] )
         VALUES ([Source].[Id], [Source].[Type], [Source].[Status], [Source].[TypeDescription], [Source].[FinancialExempt]);

SET IDENTITY_INSERT [dbo].[OrganisationType] OFF

COMMIT TRANSACTION
