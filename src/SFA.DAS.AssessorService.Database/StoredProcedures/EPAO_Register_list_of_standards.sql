CREATE PROCEDURE [dbo].[EPAO_Register_list_of_standards]
AS

SET NOCOUNT ON;

------------------------------------------------------------------------------------------------------------------------------------------------------
-- Get all the effective standards for each organisation in a sequenced order
------------------------------------------------------------------------------------------------------------------------------------------------------
SELECT
	StandardCode, EndPointAssessorName,
    row_number() over(partition by StandardCode order by StandardCode) seq 
INTO 
	#SequencedOrgStandardDetails
FROM
	(
		SELECT
			os.StandardCode, EndPointAssessorName
		FROM
			OrganisationStandard os
			INNER JOIN Organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId AND o.[Status] = 'Live'
			LEFT OUTER JOIN StandardCollation sc on os.StandardCode = sc.StandardId
		WHERE
			o.EndPointAssessorOrganisationId <> 'EPA0000'
			AND (os.effectiveTo is null OR os.EffectiveTo > GETDATE())
			AND 
			(
				JSON_Value(StandardData,'$.EffectiveTo') is null OR
				JSON_Value(StandardData,'$.EffectiveTo') > GETDATE()
			)
			AND os.[Status] = 'Live'
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
	FROM #sequencedOrgStandardDetails
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
	FROM #sequencedOrgStandardDetails
)
SELECT 
	@Dynamic_sql = 'CREATE TABLE ##' + @OrganisationStandardTableSummary + ' (StandardCode INT,' + 
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
DECLARE @StandardCode INT

DECLARE DistinctStandardCodesCursor CURSOR  
FOR SELECT DISTINCT StandardCode FROM #sequencedOrgStandardDetails
OPEN DistinctStandardCodesCursor  
FETCH NEXT FROM DistinctStandardCodesCursor INTO @StandardCode
	WHILE @@FETCH_STATUS = 0
	BEGIN
		WITH DistinctStandardCodesCTE AS
		(
			SELECT DISTINCT StandardCode
			FROM #SequencedOrgStandardDetails
		)
		SELECT 
			@Dynamic_sql = 
			'INSERT INTO ##' + @OrganisationStandardTableSummary + '(StandardCode,' + 
			(
				STUFF
				(
					(
						SELECT ',EP_AAO_' + CAST(seq AS VARCHAR) 
						FROM #sequencedOrgStandardDetails
						WHERE StandardCode = DistinctStandardCodesCTE.StandardCode
						ORDER BY StandardCode, Seq
						FOR XML PATH(''), TYPE
					).value('.','varchar(max)'),1,1,''
				)
			) + ') VALUES (' +
			(SELECT CAST(StandardCode AS VARCHAR) + ',' +
				STUFF
				(
					(
						SELECT ',''' + REPLACE(EndPointAssessorName, '''', '''''') + '''' 
						FROM #sequencedOrgStandardDetails
						WHERE StandardCode = DistinctStandardCodesCTE.StandardCode
						ORDER BY StandardCode, Seq
						FOR XML PATH(''), TYPE
					).value('.','varchar(max)'),1,1,''
				)
			) + ')'
			  FROM DistinctStandardCodesCTE
			WHERE DistinctStandardCodesCTE.StandardCode = @StandardCode

		EXECUTE sp_executesql @Dynamic_sql;
		FETCH NEXT FROM DistinctStandardCodesCursor   
		INTO @StandardCode
END   
CLOSE DistinctStandardCodesCursor;  
DEALLOCATE DistinctStandardCodesCursor; 

DROP TABLE #SequencedOrgStandardDetails

------------------------------------------------------------------------------------------------------------------------------------------------------
-- Produce the report to list all the effective standards with each organisation which has registered to assess a given standard
------------------------------------------------------------------------------------------------------------------------------------------------------
SELECT @Dynamic_sql = 
	'
		SELECT
			'''' as Trailblazer,
			isnull(JSON_VALUE(StandardData,''$.Category''),'''') [Industry Sector],
			Title as Apprentice_standards,
			StandardId as LARS_Code,
			ReferenceNumber as IFA_Code,
			isnull(JSON_VALUE(StandardData,''$.Level''),'''') [Level],' + 
			@EP_AAO_Columns +
	'	FROM
			StandardCollation sc
			LEFT OUTER JOIN ##' + @OrganisationStandardTableSummary +' sts on sts.StandardCode = sc.StandardId
		WHERE 
			ISJSON(StandardData) > 0
			and ReferenceNumber is not null
			and JSON_Value(StandardData, ''$.EffectiveFrom'') is not null
			and 
			(
				JSON_Value(StandardData,''$.EffectiveTo'') is null OR
				JSON_Value(StandardData,''$.EffectiveTo'') > GETDATE()
			)
		ORDER BY
			Title
	'
EXECUTE sp_executesql @Dynamic_sql;

------------------------------------------------------------------------------------------------------------------------------------------------------
-- Cleanup of dynamically created pivot table
------------------------------------------------------------------------------------------------------------------------------------------------------
SELECT @Dynamic_sql = 'IF OBJECT_ID(''tempdb..##' + @OrganisationStandardTableSummary + ''') IS NOT NULL DROP TABLE ##' + @OrganisationStandardTableSummary
EXECUTE sp_executesql @Dynamic_sql;
