CREATE PROCEDURE [dbo].[EPAO_Register_list_of_standards]
AS

SET NOCOUNT ON;

------------------------------------------------------------------------------------------------------------------------------------------------------
-- Get all the effective standards for each organisation in a sequenced order
------------------------------------------------------------------------------------------------------------------------------------------------------
SELECT
	StandardReference, EndPointAssessorName, Versions,
    row_number() over(partition by StandardReference, Versions order by EndPointAssessorName) Seq 
INTO 
	#SequencedOrgStandardDetails
FROM
	(
		SELECT
			os.StandardReference, EndPointAssessorName, STRING_AGG(Version,', ') WITHIN GROUP (ORDER BY CONVERT(decimal(5,2), Version) ASC) Versions
		FROM
			OrganisationStandard os
			JOIN (SELECT OrganisationStandardId, StandardUId, Version FROM OrganisationStandardVersion WHERE (EffectiveTo is null OR EffectiveTo > GETDATE()) AND [Status] = 'Live' ) osv on osv.OrganisationStandardId = os.Id
			AND osv.StandardUId LIKE os.StandardReference+'%'
			JOIN Organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId AND o.[Status] = 'Live'
			JOIN (SELECT DISTINCT TRIM(IFateReferenceNumber) IFateReferenceNumber, EffectiveFrom, EffectiveTo FROM Standards WHERE Larscode != 0 AND (EffectiveTo is null OR EffectiveTo > GETDATE()))  sc on os.StandardReference = sc.IFateReferenceNumber
		WHERE
			o.EndPointAssessorOrganisationId <> 'EPA0000'
			AND (os.EffectiveTo is null OR os.EffectiveTo > GETDATE())
			AND os.[Status] = 'Live'
		GROUP BY os.StandardReference, EndPointAssessorName
	) 
	AS OrgStandards

DECLARE @EP_AAO_Columns VARCHAR(MAX);
DECLARE @Dynamic_sql NVARCHAR(max);

------------------------------------------------------------------------------------------------------------------------------------------------------
-- Identify a list of pivot columns for each of the distinct sequence numbers for effective standards; there will be columns upto the maximum number
-- of organisations for any given standard
------------------------------------------------------------------------------------------------------------------------------------------------------
WITH DistinctEPAAOColumns AS
(
	SELECT DISTINCT Seq
	FROM #SequencedOrgStandardDetails
)
SELECT 
	@EP_AAO_Columns = 
	(
		STUFF
		(
			(
				SELECT ',EP_AAO_' + CAST(seq AS VARCHAR)
                FROM DistinctEPAAOColumns
                ORDER BY Seq
                FOR XML PATH(''), TYPE
			).value('.','varchar(max)'),1,1,''
		)
	);

------------------------------------------------------------------------------------------------------------------------------------------------------
-- Create a pivot table to hold each of the distinct sequence numbers for effective standards
--
-- The pivot table is dynamically created so must be global; generate a unique name for the table
------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @OrganisationStandardTableSummary VARCHAR(32) = REPLACE(CAST(NEWID() AS VARCHAR(36)), '-', '');

WITH DistinctEPAAOColumns AS
(
	SELECT DISTINCT Seq
	FROM #SequencedOrgStandardDetails
)
SELECT 
	@Dynamic_sql = 'CREATE TABLE ##' + @OrganisationStandardTableSummary + ' (StandardReference VARCHAR(10), Versions VARCHAR(400), ' + 
	(
		STUFF
		(
			(
				SELECT ',EP_AAO_' + CAST(seq AS VARCHAR) + ' NVARCHAR(500)'
                FROM DistinctEPAAOColumns
                ORDER BY Seq
                FOR XML PATH(''), TYPE
			).value('.','varchar(max)'),1,1,''
		)
	) + ')';

EXECUTE sp_executesql @Dynamic_sql;

------------------------------------------------------------------------------------------------------------------------------------------------------
-- Populate the pivot table with all the organisations for each standard
------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @StandardReference VARCHAR(10)
DECLARE @StandardVersions VARCHAR(400)

DECLARE DistinctStandardsCursor CURSOR  
FOR SELECT StandardReference, Versions FROM #SequencedOrgStandardDetails GROUP BY StandardReference, Versions
OPEN DistinctStandardsCursor  
FETCH NEXT FROM DistinctStandardsCursor INTO @StandardReference, @StandardVersions
	WHILE @@FETCH_STATUS = 0
	BEGIN
		WITH DistinctStandardsCTE AS
		(
			SELECT StandardReference, Versions
			FROM #SequencedOrgStandardDetails
			GROUP BY StandardReference, Versions
		)
		SELECT 
			@Dynamic_sql = 
			'INSERT INTO ##' + @OrganisationStandardTableSummary + '(StandardReference, Versions, ' + 
			(
				STUFF
				(
					(
						SELECT ',EP_AAO_' + CAST(seq AS VARCHAR) 
						FROM #SequencedOrgStandardDetails
						WHERE StandardReference = DistinctStandardsCTE.StandardReference AND Versions = DistinctStandardsCTE.Versions
						ORDER BY StandardReference, Seq
						FOR XML PATH(''), TYPE
					).value('.','varchar(max)'),1,1,''
				)
			) + ') VALUES (' +
			(SELECT ''''+StandardReference + ''','''+ Versions + ''',' +
				STUFF
				(
					(
						SELECT ',''' + REPLACE(EndPointAssessorName, '''', '''''') + '''' 
						FROM #SequencedOrgStandardDetails
						WHERE StandardReference = DistinctStandardsCTE.StandardReference AND Versions = DistinctStandardsCTE.Versions
						ORDER BY StandardReference, Seq
						FOR XML PATH(''), TYPE
					).value('.','varchar(max)'),1,1,''
				)
			) + ')'
			  FROM DistinctStandardsCTE
			WHERE DistinctStandardsCTE.StandardReference = @StandardReference AND DistinctStandardsCTE.Versions = @StandardVersions


		EXECUTE sp_executesql @Dynamic_sql;
		FETCH NEXT FROM DistinctStandardsCursor   
		INTO @StandardReference, @StandardVersions
END   
CLOSE DistinctStandardsCursor;  
DEALLOCATE DistinctStandardsCursor; 


DROP TABLE #SequencedOrgStandardDetails

------------------------------------------------------------------------------------------------------------------------------------------------------
-- Produce the report to list all the effective standards with each organisation which has registered to assess a given standard
------------------------------------------------------------------------------------------------------------------------------------------------------
SELECT @Dynamic_sql = 
'
    
    SELECT '''' Trailblazer
	, [Industry Sector] 
	, Apprentice_standards
	, Larscode [Standard Code]
	, IFateReferenceNumber [Standard reference]
	, Level
	, Versions [Standard version]
	, ' + @EP_AAO_Columns +
'   
	FROM (
    SELECT 
      MAX(CASE WHEN latestcheck = 1 THEN TrailBlazerContact ELSE NULL END) Trailblazer 
    , MAX(CASE WHEN latestcheck = 1 THEN Route ELSE NULL END) [Industry Sector] 
    , MAX(CASE WHEN latestcheck = 1 THEN Title ELSE NULL END) Apprentice_standards 
    , Larscode 
    , IFateReferenceNumber 
    , STRING_AGG(Version,'','') WITHIN GROUP (ORDER BY CONVERT(decimal(5,2), Version) ASC) [Available Versions]
    , MAX(CASE WHEN latestcheck = 1 THEN Level ELSE NULL END) Level 
    FROM (
    SELECT TRIM(IFateReferenceNumber) IFateReferenceNumber, Larscode, Title, Level, Version, TrailBlazerContact, Route 
    , ROW_NUMBER() OVER (PARTITION BY IFateReferenceNumber, Larscode ORDER BY CONVERT(decimal(5,2), Version) DESC) latestcheck
    FROM Standards 
    WHERE LarsCode != 0 
    AND IFateReferenceNumber IS NOT NULL
    AND EffectiveFrom IS NOT NULL
    AND (EffectiveTo IS NULL OR EffectiveTo > GETDATE() )
    ) ab1
    GROUP BY IFateReferenceNumber, Larscode
    ) ab2
    LEFT OUTER JOIN ##' + @OrganisationStandardTableSummary +' sts on sts.StandardReference = ab2.IFateReferenceNumber 
		ORDER BY
			Apprentice_standards
'


EXECUTE sp_executesql @Dynamic_sql;

------------------------------------------------------------------------------------------------------------------------------------------------------
-- Cleanup of dynamically created pivot table
------------------------------------------------------------------------------------------------------------------------------------------------------
SELECT @Dynamic_sql = 'IF OBJECT_ID(''tempdb..##' + @OrganisationStandardTableSummary + ''') IS NOT NULL DROP TABLE ##' + @OrganisationStandardTableSummary
EXECUTE sp_executesql @Dynamic_sql;


