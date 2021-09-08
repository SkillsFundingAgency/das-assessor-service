-- Data for Opportunity Finder
-- 1. Approved all Standards with Active Learners and Registered EPAOs counts
-- 2. Drill by Standard, 
--	a. Header details (API call to Standard Collations)
--	b. Filtered by Standard; sum of Active Learners, sum of Learners without an EPAO (based on ILR data), sum of Completed Assessments
--	c. List of EPAOS by region, Active Learners by Region and Completed Assessments by Region 
--	   with details of all EPAOs (can be over 30!)
--
CREATE PROCEDURE OppFinder_Update_StandardSummary
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
		StandardName nvarchar(500),
		StandardReference nvarchar(10)
	) 
	
	INSERT INTO @Exclusions(StandardName, StandardReference)
	EXEC OppFinder_Exclusions 

	DECLARE @StandardsCore TABLE
	(
		 StandardCode INT NULL, 
		 StandardReference nvarchar(10) NULL,
		 StandardName nvarchar(500) NOT NULL,
		 StandardLevel INT NULL,
		 Duration INT NULL,
		 Sector nvarchar(500) NOT NULL,
		 Versions nvarchar(500) NULL
	);

	INSERT INTO @StandardsCore (StandardCode, StandardReference,StandardName, StandardLevel, Sector, Versions)
	SELECT StandardCode, stv.StandardReference, stv.StandardName, StandardLevel, Sector, Versions
	FROM (
		SELECT st1.LarsCode StandardCode
			  ,st1.IFateReferenceNumber StandardReference
			  ,Title StandardName
			  ,Level StandardLevel
			  ,Route Sector
			  ,AllVersions Versions
			  ,ROW_NUMBER() OVER(PARTITION BY st1.IFateReferenceNumber ORDER BY VersionMajor DESC, VersionMinor DESC) AS RowNumber
		FROM Standards st1
		JOIN ( SELECT IFateReferenceNumber, STRING_AGG(CAST(Version AS VARCHAR(500)), ',') WITHIN GROUP (ORDER BY VersionMajor , VersionMinor) AS AllVersions 
			   FROM Standards 
			   WHERE VersionApprovedForDelivery IS NOT NULL
			   GROUP BY IFateReferenceNumber ) sv1 ON sv1.IFateReferenceNumber = st1.IFateReferenceNumber
		 WHERE 1=1
		 -- only one version of a Standard should be "Approved for delivery" at any one time, and it should be the latest, but 
		   AND ISNULL(Status, '') = 'Approved for delivery' 
		   AND ISNULL(IntegratedDegree, '') <> 'integrated degree'
		   AND (EffectiveTo IS NULL OR EffectiveTo > GETDATE() )
		   -- When LARS set LastDateStarts to EffectiveFrom Date this is because there is no EPAO for this standard, so we want EPAOs to see the Opportunity!
		   AND (LastDateStarts IS NULL OR LastDateStarts = EffectiveFrom)
	) stv 
	LEFT JOIN @Exclusions ex1 ON ex1.StandardReference = stv.StandardReference
	WHERE RowNumber = 1
	  AND ex1.StandardName IS NULL;

	BEGIN TRY;
	
	DELETE FROM StandardSummary;
	
	INSERT INTO StandardSummary
	-- combine results FROM 4 subqueries
	SELECT ac.StandardCode, ac.StandardReference, ac.StandardName, ac.StandardLevel, ac.Sector, 
		   Total.Region, Total.Ordering, Total.Learners, Total.Assessments, Total.TotalEPAOs, Total.EndPointAssessors, 
		   Total.EndPointAssessorList, GETDATE() UpdatedAt, ac.Versions
	FROM @StandardsCore ac		  
	JOIN (

	SELECT StandardReference, Region, Ordering, SUM(Learners) Learners, SUM(Assessments) Assessments, SUM(TotalEPAOs) TotalEPAOs,SUM(EndPointAssessors) EndPointAssessors, 
			ISNULL(MAX(EndPointAssessorList),'') EndPointAssessorList
	FROM (
		-- Active Learners by Standard and Region and version
		SELECT ab1.StandardReference
			  ,ISNULL(Area,'Other') Region, ISNULL(Ordering,10) Ordering, Learners
			  ,0 Assessments
			  ,0 TotalEPAOs
			  ,0 EndPointAssessors
			  ,NULL EndPointAssessorList
		FROM (
			SELECT COUNT(*) Learners,  StandardReference, ISNULL(pc1.DeliveryAreaId ,0) DeliveryAreaId 
			FROM (
				-- learner data that is in the future (has not been completed or withdrawn and does not have a cert)
				SELECT le1.StandardReference, le1.DelLocPostCode
				FROM Learner le1 
				LEFT JOIN Certificates ce1 ON ce1.StandardCode = le1.StdCode and ce1.Uln = le1.Uln 
				WHERE ce1.Uln IS NULL
				AND le1.FundingModel != 99
				AND le1.CompletionStatus = 1
				AND le1.EstimatedEndDate >= DATEADD(month,-6,GETDATE())
			) il2
			LEFT JOIN PostCodeRegion pc1 on pc1.PostCodePrefix = dbo.OppFinder_GetPostCodePrefix(DelLocPostCode)
			GROUP BY StandardReference, ISNULL(pc1.DeliveryAreaId ,0) 
		) ab1
		LEFT JOIN DeliveryArea de1 on de1.Id = ab1.DeliveryAreaId

		UNION ALL

		-- EPAOs by Region
		SELECT StandardReference, Region, Ordering
			  ,0 Learners
			  ,0 Assessments
			  ,TotalEPAOs
			  ,EndPointAssessors
			  ,EndPointAssessorList
		FROM (

			SELECT os1.StandardReference ,ISNULL(Area,'Other') Region, de1.Ordering, TotalEPAOs, COUNT(*) EndPointAssessors
					,'{"EPAOS":['+STRING_AGG (CAST('"'+EndPointAssessorName+'"' as NVARCHAR(MAX)), ',') WITHIN GROUP (ORDER BY EndPointAssessorName ASC)+']}' AS EndPointAssessorList 
			FROM (
				SELECT COUNT(*) OVER (PARTITION BY os2.StandardReference) TotalEPAOs,os2.* 
				FROM OrganisationStandard os2 
				JOIN (SELECT DISTINCT OrganisationStandardId 
						FROM OrganisationStandardVersion 
					   WHERE status = 'Live' AND (EffectiveTo IS NULL OR EffectiveTo > GETDATE() ) 
					 ) osv ON os2.id = osv.OrganisationStandardId
				WHERE os2.Status = 'Live' 
				  AND (os2.EffectiveTo IS NULL OR os2.EffectiveTo > GETDATE() ) 
			) os1 
			JOIN Organisations og1 ON og1.EndPointAssessorOrganisationId = os1.EndPointAssessorOrganisationId
			JOIN OrganisationStandardDeliveryArea od1 ON os1.Id = od1.OrganisationStandardId
			JOIN DeliveryArea de1 on de1.Id = od1.DeliveryAreaId
			GROUP BY os1.StandardReference ,TotalEPAOs, ISNULL(Area,'Other'), de1.Ordering

		) epaos

		UNION ALL

		-- All Regions by All Active Standards
		SELECT sc1.StandardReference
				,ISNULL(Area,'Other') Region, de1.Ordering, 0 Learners, 0 Assessments, 0 TotalEPAOs, 0 EndPointAssessors, NULL EndPointAssessorList
		FROM (SELECT DISTINCT IFateReferenceNumber StandardReference FROM Standards WHERE VersionApprovedForDelivery IS NOT NULL)  sc1
		CROSS JOIN DeliveryArea de1

		UNION ALL

		-- Assessments
		SELECT od1.StandardReference
				,ISNULL(Area,'Other') Region, ISNULL(Ordering,10) Ordering, 0 Learners, COUNT(*) Assessments, 0 TotalEPAOs, 0 EndPointAssessors, NULL EndPointAssessorList
		FROM (

			SELECT JSON_VALUE(ce1.CertificateData,'$.StandardReference') StandardReference,  ISNULL(ISNULL(le1.DelLocPostCode, JSON_VALUE(ce1.CertificateData,'$.ContactPostCode')),'ZZ99 9ZZ') DelLocPostCode
			  FROM Certificates ce1
			LEFT JOIN Learner le1 ON le1.StdCode = ce1.StandardCode AND le1.Uln = ce1.Uln
			WHERE  ce1.Status NOT IN ('Deleted','Draft')
			  AND IsPrivatelyFunded = 0

		) od1
		LEFT JOIN PostCodeRegion pc1 on pc1.PostCodePrefix = dbo.OppFinder_GetPostCodePrefix(od1.DelLocPostCode)
		LEFT JOIN DeliveryArea de1 on de1.Id = pc1.DeliveryAreaId
		GROUP BY od1.StandardReference, ISNULL(Area,'Other'), ISNULL(Ordering,10) 
	) ab1
	WHERE NOT (Region = 'Other' AND Learners = 0)
	GROUP BY StandardReference, Region, Ordering
	) Total On Total.StandardReference = ac.StandardReference
	ORDER BY StandardReference, Ordering
	
	
	-- populate the StandardVersionSummary table
	DELETE FROM StandardVersionSummary;
	
	INSERT INTO StandardVersionSummary
	(StandardCode, StandardReference, Version, ActiveApprentices, CompletedAssessments, EndPointAssessors, UpdatedAt)
	SELECT st1.Larscode StandardCode
		,st1.IfateReferenceNumber StandardReference
		,st1.Version
		,ActiveApprentices
		,CompletedAssessments
		,EndPointAssessors
		,GETDATE() UpdatedAt
	FROM Standards st1 
	JOIN @StandardsCore ac ON ac.StandardReference = st1.IfateReferenceNumber
	JOIN (
		SELECT StandardUId
			  ,MAX(ActiveApprentices) ActiveApprentices
			  ,MAX(CompletedAssessments) CompletedAssessments
			  ,MAX(EndPointAssessors) EndPointAssessors
		FROM (	
			-- EPAOs 
			SELECT osv.StandardUId, COUNT(DISTINCT EndPointAssessorOrganisationId) AS EndPointAssessors, 0 CompletedAssessments, 0 ActiveApprentices
			FROM OrganisationStandardVersion osv
			INNER JOIN OrganisationStandard os ON osv.OrganisationStandardId = os.Id
			WHERE os.Status = 'Live' 
			  AND osv.Status = 'Live'
			  AND (os.EffectiveTo IS NULL OR os.EffectiveTo > GETDATE()) 
			  AND (osv.EffectiveTo IS NULL OR osv.EffectiveTo > GETDATE())
			GROUP BY osv.StandardUId

			UNION ALL
			
			-- Assessments
			SELECT StandardUId, 0 EndPointAssessors, COUNT(*) AS CompletedAssessments, 0 ActiveApprentices
			FROM Certificates 
			WHERE IsPrivatelyFunded = 0
			  AND Status NOT IN ('Deleted','Draft')
			GROUP BY StandardUId
			
			UNION ALL

			-- learner data that is in the future (has not been completed or withdrawn and does not have a cert)
			SELECT le1.StandardUId, 0 EndPointAssessors, 0 AS CompletedAssessments, COUNT(*) AS ActiveApprentices
			FROM Learner le1
			LEFT JOIN Certificates ce1 ON ce1.StandardCode = le1.StdCode and ce1.Uln = le1.Uln 
			WHERE ce1.Uln IS NULL
			  AND le1.FundingModel != 99
			  AND le1.CompletionStatus = 1
			  AND le1.EstimatedEndDate >= DATEADD(month,-6,GETDATE())
			GROUP BY le1.StandardUId

			UNION ALL

			-- all versions of all standards
			SELECT StandardUId, 0 EndPointAssessors, 0 AS CompletedAssessments, 0 AS ActiveApprentices
			FROM Standards
			WHERE VersionApprovedForDelivery IS NOT NULL 
		
		) ct1 GROUP BY StandardUid
		
	) vt1 ON vt1.StandardUId = st1.StandardUId
	
  
	END TRY
	BEGIN CATCH;
		-- Some basic error handling
		ROLLBACK TRANSACTION T1;
		SELECT @Error_Code = ERROR_NUMBER(), @Error_Message = ERROR_MESSAGE(), @Error_Severity = ERROR_SEVERITY();
		raiserror (@Error_Message, @Error_Severity,@Error_Code);
	END CATCH;

	 IF @Error_Code = 0 AND XACT_STATE() = 1 COMMIT TRANSACTION T1;
END