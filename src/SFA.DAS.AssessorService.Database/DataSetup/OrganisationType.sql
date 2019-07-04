/*
	Setup data for the [OrganisationType] table
*/
BEGIN TRANSACTION

CREATE TABLE #OrganisationType
(
	[Id] INT NOT NULL,
	[Type] NVARCHAR(256) NOT NULL,
	[Status] NVARCHAR(10) NOT NULL,
	[TypeDescription] NVARCHAR(500) NULL,
	[FinancialExempt] BIT NOT NULL
)

INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(1,'Awarding Organisations','Live','Awarding organisations', 0)
INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(2,'Assessment Organisations','Live','Assessment organisations', 0)
INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(3,'Trade Body','Live','Trade body', 0)
INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(4,'Professional Body','Live','Professional body - approved by the relevant council', 0)
INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(5,'HEI','Live','Higher education institute (HEI) monitored and supported by the Office for Students (OfS)', 1)
INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(6,'NSA or SSC','Live','National skills academy or sector skills council', 0)
INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(7,'Training Provider','Live','Training provider - including HEI not in England', 0)
INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(8,'Other','Deleted',NULL, 0)
INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(9,'Public Sector','Live','Incorporated as a public sector body', 1)
INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(10,'College','Live','General further education (GFE) college currently receiving funding from the ESFA, 6th form or further education (FE) college', 1)
INSERT INTO #OrganisationType ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES(11,'Academy or Free School','Live','Academy or Free school registered with the ESFA', 1)

SET IDENTITY_INSERT OrganisationType ON

MERGE OrganisationType AS Target
USING
(
	SELECT [Id], [Type], [Status], [TypeDescription], [FinancialExempt]
	FROM #OrganisationType
)
AS Source (Id, Type, Status, TypeDescription, FinancialExempt)
ON Target.Id = Source.Id
WHEN MATCHED THEN
	UPDATE SET
		[Type] = Source.[Type],
		[Status] = Source.[Status],
		[TypeDescription] = Source.[TypeDescription],
		[FinancialExempt] = Source.[FinancialExempt]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Id], [Type], [Status], [TypeDescription], [FinancialExempt]) VALUES([Id], [Type], [Status], [TypeDescription], [FinancialExempt])
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

SET IDENTITY_INSERT OrganisationType OFF

DROP TABLE #OrganisationType

COMMIT TRANSACTION