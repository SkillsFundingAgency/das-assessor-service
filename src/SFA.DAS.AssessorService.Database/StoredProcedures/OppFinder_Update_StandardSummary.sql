-- Data for Opportunity Finder
-- 1. Approved all Standards with Active Learners and Registered EPAOs counts
-- 2. Drill by Standard, 
--	a. Header details (API call to Standard Collations)
--	b. Filtered by Standard; sum of Active Learners, sum of Learners without an EPAO (based on ILR data), sum of Completed Assessments
--	c. List of EPAOS by region, Active Learners by Region and Completed Assessments by Region 
--	   with details of all EPAOs (can be over 30!)
--
CREATE PROCEDURE [OppFinder_Update_StandardSummary]
AS

SET NOCOUNT ON;

DECLARE @Error_Code INT = 0
		,@Error_Message VARCHAR(MAX)
		,@Error_Severity INT = 0

BEGIN
	BEGIN TRANSACTION T1;
	
	-- there are some specifically excluded Standards
	DECLARE @Exclusions TABLE
	(
		[StandardName] nvarchar(500),
		[StandardReference] nvarchar(10)
	) 
	
	INSERT INTO @Exclusions(StandardName, StandardReference)
	EXEC OppFinder_Exclusions 

	BEGIN TRY;
	
	DELETE FROM [StandardSummary];
	
	INSERT INTO [StandardSummary]
	-- combine results FROM 4 subqueries
	SELECT StandardCode, Total.StandardReference, StandardLevel, NULL as Title, Sector, Region, Ordering, SUM(Learners) Learners, SUM(Assessments) Assessments, SUM(TotalEPAOs) TotalEPAOs,SUM(EndPointAssessors) EndPointAssessors, ISNULL(MAX(EndPointAssessorList),'') EndPointAssessorList, GETDATE() UpdatedAt, NULL as Versions
	FROM (

	-- Active Learners by Standard and Region
		SELECT StdCode StandardCode, ReferenceNumber StandardReference, JSON_VALUE(StandardData, '$.Category') Sector, JSON_VALUE(StandardData,'$.Level') StandardLevel
				,ISNULL(Area,'Other') Region, ISNULL(Ordering,10) Ordering, Learners, 0 Assessments, 0 TotalEPAOs, 0 EndPointAssessors, NULL EndPointAssessorList
		FROM (
			SELECT COUNT(*) Learners,  StdCode, ISNULL(pc1.DeliveryAreaId ,0) DeliveryAreaId 
			FROM (
			-- ILR data that is in the future (has not been completed or withdrawn and does not have a cert)
				SELECT il1.StdCode, il1.DelLocPostCode
				FROM Ilrs il1 
				JOIN (SELECT StandardId, CONVERT(numeric,JSON_VALUE([StandardData],'$.Duration')) Duration 
						FROM [StandardCollation] 
						WHERE 1=1 
						AND [dbo].[OppFinder_Is_Approved_StandardStatus](StandardData) = 1
						AND (
							JSON_VALUE(StandardData,'$.EffectiveTo') IS NULL OR
							JSON_VALUE(StandardData,'$.EffectiveTo') > GETDATE() OR
							JSON_VALUE(StandardData,'$.LastDateForNewStarts') IS NULL 
						)
				) st1 ON st1.StandardId = il1.StdCode
				LEFT JOIN Organisations og1 ON og1.EndPointAssessorOrganisationId = il1.EpaOrgId
				LEFT JOIN Certificates ce1 ON ce1.StandardCode = il1.StdCode and ce1.Uln = il1.Uln 
				WHERE ce1.Uln IS NULL
				AND il1.CompletionStatus = 1
				AND (CASE WHEN il1.PlannedEndDate > GETDATE() THEN EOMONTH(il1.PlannedEndDate) ELSE EOMONTH(DATEADD(month, st1.Duration, il1.LearnStartDate)) END) >= DATEADD(month,-3,GETDATE())
			) il2
			LEFT JOIN PostCodeRegion pc1 on pc1.PostCodePrefix = dbo.OppFinder_GetPostCodePrefix(DelLocPostCode)
			GROUP BY StdCode, ISNULL(pc1.DeliveryAreaId ,0) 
		) ab1
		LEFT JOIN DeliveryArea de1 on de1.Id = ab1.DeliveryAreaId
		JOIN StandardCollation sc1 on sc1.StandardId = ab1.StdCode


		UNION 

		-- EPAOs by Region
		SELECT sc1.StandardId StandardCode, sc1.ReferenceNumber StandardReference, JSON_VALUE(sc1.StandardData, '$.Category') Sector, JSON_VALUE(sc1.StandardData, '$.Level') StandardLevel 
				,os1.Region, os1.Ordering, 0 Learners, 0 Assessments, os1.TotalEPAOs, os1.EndPointAssessors, os1.EndPointAssessorList
		FROM StandardCollation sc1
		JOIN (
			SELECT os1.StandardCode ,ISNULL(Area,'Other') Region, de1.Ordering, TotalEPAOs, COUNT(*) EndPointAssessors
					,'{"EPAOS":['+STRING_AGG (CAST('"'+EndPointAssessorName+'"' as NVARCHAR(MAX)), ',') WITHIN GROUP (ORDER BY EndPointAssessorName ASC)+']}' AS EndPointAssessorList 
			FROM (
				SELECT COUNT(*)  OVER (PARTITION BY os2.StandardCode) TotalEPAOs,os2.* 
				FROM OrganisationStandard os2 
				WHERE os2.Status = 'Live' 
				AND (os2.EffectiveTo IS NULL OR os2.EffectiveTo > GETDATE()) 
			) os1 
			JOIN Organisations og1 ON og1.EndPointAssessorOrganisationId = os1.EndPointAssessorOrganisationId
			JOIN OrganisationStandardDeliveryArea od1 ON os1.Id = od1.OrganisationStandardId
			JOIN DeliveryArea de1 on de1.Id = od1.DeliveryAreaId
			GROUP BY os1.StandardCode ,TotalEPAOs, ISNULL(Area,'Other'), de1.Ordering
		) os1 ON sc1.StandardId = os1.StandardCode
		WHERE 1=1 
		AND [dbo].[OppFinder_Is_Approved_StandardStatus](StandardData) = 1
		AND (
			JSON_VALUE(StandardData,'$.EffectiveTo') IS NULL OR
			JSON_VALUE(StandardData,'$.EffectiveTo') > GETDATE() OR
			JSON_VALUE(StandardData,'$.LastDateForNewStarts') IS NULL 
		) 


		UNION 

		-- All Regions by All Active Standards
		SELECT sc1.StandardId StandardCode, sc1.ReferenceNumber StandardReference, JSON_VALUE(sc1.StandardData, '$.Category') Sector, JSON_VALUE(sc1.StandardData, '$.Level') StandardLevel 
				,ISNULL(Area,'Other') Region, de1.Ordering, 0 Learners, 0 Assessments, 0 TotalEPAOs, 0 EndPointAssessors, NULL EndPointAssessorList
		FROM StandardCollation sc1
		CROSS JOIN DeliveryArea de1
		WHERE 1=1 
		AND [dbo].[OppFinder_Is_Approved_StandardStatus](StandardData) = 1
		AND (
			JSON_VALUE(StandardData,'$.EffectiveTo') IS NULL OR
			JSON_VALUE(StandardData,'$.EffectiveTo') > GETDATE() OR
			JSON_VALUE(StandardData,'$.LastDateForNewStarts') IS NULL 
		)


		UNION

	-- Assessments
		SELECT StandardCode,  ReferenceNumber StandardReference, JSON_VALUE(StandardData, '$.Category') Sector, JSON_VALUE(StandardData,'$.Level') StandardLevel
				,ISNULL(Area,'Other') Region, ISNULL(Ordering,10) Ordering, 0 Learners, COUNT(*) Assessments, 0 TotalEPAOs, 0 EndPointAssessors, NULL EndPointAssessorList
		FROM (
			SELECT ce1.StandardCode,  ISNULL(ISNULL(il1.DelLocPostCode, JSON_VALUE(ce1.CertificateData,'$.ContactPostCode')),'ZZ99 9ZZ') DelLocPostCode
			FROM [Certificates] ce1
			LEFT JOIN Ilrs il1 ON il1.StdCode = ce1.StandardCode AND il1.Uln = ce1.Uln
			WHERE 1=1
			AND IsPrivatelyFunded = 0
			AND ce1.[Status] NOT IN ('Deleted','Draft')
		) od1
		LEFT JOIN PostCodeRegion pc1 on pc1.PostCodePrefix = dbo.OppFinder_GetPostCodePrefix(od1.DelLocPostCode)
		LEFT JOIN DeliveryArea de1 on de1.Id = pc1.DeliveryAreaId
		JOIN StandardCollation sc1 On sc1.StandardId = od1.StandardCode
		WHERE 1=1
		AND [dbo].[OppFinder_Is_Approved_StandardStatus](StandardData) = 1
		AND	(
			JSON_VALUE(StandardData,'$.EffectiveTo') IS NULL OR
			JSON_VALUE(StandardData,'$.EffectiveTo') > GETDATE() OR
			JSON_VALUE(StandardData,'$.LastDateForNewStarts') IS NULL 
		)
		GROUP BY StandardCode,  ReferenceNumber, Title, JSON_VALUE(StandardData, '$.Category'), JSON_VALUE(StandardData,'$.Level'), ISNULL(Area,'Other'), ISNULL(Ordering,10) 


	) Total
	-- exclude specific standard references
	LEFT JOIN @Exclusions ex1 ON ex1.StandardReference = Total.StandardReference
	WHERE NOT (Region = 'Other' AND Learners = 0)
	AND ex1.StandardName IS NULL
	GROUP BY StandardCode, Total.StandardReference, Sector, StandardLevel, Region,Ordering 
	ORDER BY StandardCode, Ordering;
	
	-- populate the title and versions columns
	WITH standardversions AS
	(
		SELECT [IFateReferenceNumber], STRING_AGG(CAST([Version] AS VARCHAR(50)), ',') WITHIN GROUP ( ORDER BY [Version] ASC) AS [Versions] 
		FROM (SELECT [IFateReferenceNumber], [Version]
				FROM [Standards]) s
		GROUP BY [IFateReferenceNumber]
	),
	latesttitles AS
	(
		SELECT [IFateReferenceNumber], [Title] 
		FROM (SELECT [IFateReferenceNumber], [Title], ROW_NUMBER() OVER(PARTITION BY [IFateReferenceNumber] ORDER BY [Version] DESC) AS RowNum
				FROM [Standards]) s
		WHERE RowNum = 1
	)
	UPDATE ss
	SET ss.Versions = sv.Versions, ss.StandardName = st.Title
	FROM [StandardSummary] ss
		LEFT JOIN standardversions sv ON sv.IFateReferenceNumber = ss.[StandardReference] 
		LEFT JOIN latesttitles st ON st.IFateReferenceNumber = ss.[StandardReference] 

	
	-- populate the standardversionsummary table
	DELETE FROM [StandardVersionSummary];

	WITH endpointassessors AS 
	(
		SELECT os.[StandardCode], os.[StandardReference], osv.[Version], COUNT(*) AS EndPointAssessors
		FROM OrganisationStandardVersion osv
			INNER JOIN OrganisationStandard os ON osv.[OrganisationStandardId] = os.[Id]
		WHERE os.Status = 'Live' 
			AND osv.Status = 'Live'
			AND (os.EffectiveTo IS NULL OR os.EffectiveTo > GETDATE()) 
			AND (osv.EffectiveTo IS NULL OR osv.EffectiveTo > GETDATE())
		GROUP BY os.[StandardCode], os.[StandardReference], osv.[Version]
	),
	completedassessments AS
	(
		SELECT ce.[StandardCode], s.[IFateReferenceNumber] AS StandardReference, s.[Version], COUNT(*) AS CompletedAssessments
		FROM Certificates ce
			INNER JOIN Standards s ON s.StandardUId = ce.StandardUId
		WHERE IsPrivatelyFunded = 0
			AND ce.[Status] NOT IN ('Deleted','Draft')
		GROUP BY ce.[StandardCode], s.[IFateReferenceNumber], s.[Version]
	),
	activeapprentices AS
	(
		SELECT s.[StandardCode], s.[StandardReference], s.[Version], Count(*) AS ActiveApprentices 
		FROM ilrs i
			INNER JOIN (SELECT [LarsCode] AS StandardCode, [IFateReferenceNumber] AS StandardReference, MIN([Version]) AS Version
						FROM Standards
						GROUP BY [LarsCode], [IFateReferenceNumber]
						Having Count(*) = 1) s ON s.StandardCode = i.StdCode
		GROUP BY s.[StandardCode], s.[StandardReference], s.[Version]
	)
	INSERT INTO [StandardVersionSummary]
	SELECT s.[LarsCode] AS StandardCode, 
			s.[IFateReferenceNumber] AS StandardReference, 
			s.[Version], 
			ISNULL(aa.[ActiveApprentices], 0) AS ActiveApprentices, 
			ISNULL(ca.[CompletedAssessments], 0) AS CompletedAssessments,
			ISNULL(ea.[EndPointAssessors], 0) AS EndPointAssessors, 
			GETDATE() AS UpdatedAt
	FROM 
		Standards s
		LEFT JOIN endpointassessors ea ON ea.[StandardCode] = s.[LarsCode] AND ea.[StandardReference] = s.[IFateReferenceNumber] AND ea.[Version] = s.[Version]
		LEFT JOIN completedassessments ca ON ca.[StandardCode] = s.[LarsCode] AND ca.[StandardReference] = s.[IFateReferenceNumber] AND ca.[Version] = s.[Version]
		LEFT JOIN activeapprentices aa ON aa.[StandardCode] = s.[LarsCode] AND aa.[StandardReference] = s.[IFateReferenceNumber] AND aa.[Version] = s.[Version]
		INNER JOIN [StandardCollation] sc ON sc.[ReferenceNumber] = s.[IFateReferenceNumber] 
	WHERE
		 s.[VersionApprovedForDelivery] IS NOT NULL
		 AND [dbo].[OppFinder_Is_Approved_StandardStatus](StandardData) = 1
		 AND (
			JSON_VALUE(StandardData,'$.EffectiveTo') IS NULL OR
			JSON_VALUE(StandardData,'$.EffectiveTo') > GETDATE() OR
			JSON_VALUE(StandardData,'$.LastDateForNewStarts') IS NULL 
		);

	END TRY
	BEGIN CATCH;
		-- Some basic error handling
		ROLLBACK TRANSACTION T1;
		SELECT @Error_Code = ERROR_NUMBER(), @Error_Message = ERROR_MESSAGE(), @Error_Severity = ERROR_SEVERITY();
		raiserror (@Error_Message, @Error_Severity,@Error_Code);
	END CATCH;

	 IF @Error_Code = 0 AND XACT_STATE() = 1 COMMIT TRANSACTION T1;
END