-- Data for Opportunity Finder
-- 1. Approved all Standards with Active Learners and Registered EPAOs counts
-- 2. Drill by Standard, 
--    a. Header details (API call to Standard Collations)
--    b. Filtered by Standard; sum of Active Learners, sum of Learners without an EPAO (based on ILR data), sum of Completed Assessments
--    c. List of EPAOS by region, Active Learners by Region and Completed Assessments by Region 
--       with details of all EPAOs (can be over 30!)
--
CREATE PROCEDURE [Data_Summaries]
AS

SET NOCOUNT ON;

DECLARE @Error_Code INT = 0
        ,@Error_Message     VARCHAR(MAX)

BEGIN
    BEGIN TRANSACTION T1;
    
    BEGIN TRY;
    
    DELETE FROM [StandardSummary];
    
    INSERT INTO [StandardSummary]
    -- combine results from 4 subqueries
    SELECT StandardCode, StandardReference, StandardName, StandardLevel, Sector, Region, Ordering, SUM(Learners) Learners, SUM(Assessments) Assessments, SUM(TotalEPAOs) TotalEPAOs,SUM(EndPointAssessors) EndPointAssessors, ISNULL(MAX(EndPointAssessorList),'') EndPointAssessorList, GETDATE() UpdatedAt
    FROM (

    -- Active Learners by Standard and Region
    SELECT StdCode StandardCode, ReferenceNumber StandardReference, Title StandardName, JSON_VALUE(sc1.StandardData, '$.Category') Sector, JSON_VALUE(StandardData,'$.Level') StandardLevel,
    ISNULL(Area,'Other') Region, ISNULL(Ordering,10) Ordering, Learners, 0 Assessments, 0 TotalEPAOs, 0 EndPointAssessors, NULL EndPointAssessorList
    FROM (
    SELECT COUNT(*) Learners,  StdCode, ISNULL(pc1.DeliveryAreaId ,0) DeliveryAreaId 
    from (
    -- ILR data that is in the future (has not been completed or withdrawn and does not have a cert)
    SELECT il1.*
    FROM Ilrs il1 
    JOIN (SELECT StandardId, CONVERT(numeric,JSON_VALUE([StandardData],'$.Duration')) Duration FROM [dbo].[StandardCollation] 
    WHERE json_value(StandardData ,'$.IfaStatus') = 'Approved for delivery' AND
    (
        JSON_VALUE(StandardData,'$.EffectiveTo') is null OR
        JSON_VALUE(StandardData,'$.EffectiveTo') > GETDATE() OR
        JSON_VALUE(StandardData,'$.LastDateForNewStarts') is null 
    )
    ) std ON std.StandardId = il1.StdCode
    LEFT JOIN Organisations og1 ON og1.EndPointAssessorOrganisationId = il1.EPAOrgid
    LEFT JOIN Certificates ce1 ON ce1.StandardCode = il1.stdcode and ce1.Uln = il1.uln 
    WHERE ce1.Uln IS NULL
    AND il1.CompletionStatus = 1
    AND (CASE WHEN il1.PlannedEndDate > GETDATE() THEN EOMONTH(il1.PlannedEndDate) ELSE EOMONTH(DATEADD(month, std.Duration, il1.LearnStartDate)) END) >= dateadd(month,-3,GETDATE())
    ) il2
    LEFT JOIN PostCodeRegion pc1 on pc1.PostCodePrefix = LEFT(DelLocPostCode,(PATINDEX('%[0-9]%',ISNULL(DelLocPostCode,'ZZ'))-1))
    GROUP BY StdCode, ISNULL(pc1.DeliveryAreaId ,0) 
    ) ab1
    LEFT JOIN [DeliveryArea] de1 on de1.Id = ab1.DeliveryAreaId
    join StandardCollation sc1 on sc1.StandardId = ab1.StdCode

    UNION 

    -- EPAOs by Region
    SELECT sc1.StandardId StandardCode, sc1.ReferenceNumber StandardReference, sc1.Title 'StandardName',JSON_VALUE(sc1.StandardData, '$.Category') Sector, JSON_VALUE(sc1.StandardData, '$.Level') StandardLevel 
    ,os1.Region, os1.Ordering, 0 Learners, 0 Assessments, os1.TotalEPAOs, os1.EndPointAssessors, os1.EndPointAssessorList
    FROM StandardCollation sc1
    JOIN (
    SELECT os1.StandardCode ,ISNULL(Area,'Other') Region, de1.Ordering, TotalEPAOs, COUNT(*) EndPointAssessors
    , '{"EPAOS":['+STRING_AGG (CAST('"'+EndPointAssessorName+'"' as NVARCHAR(MAX)), ',') WITHIN GROUP (ORDER BY EndPointAssessorName ASC)+']}' AS EndPointAssessorList 
    FROM (SELECT COUNT(*)  OVER (PARTITION BY os2.StandardCode) TotalEPAOs,os2.* FROM OrganisationStandard os2 
    WHERE os2.Status = 'Live' AND (os2.EffectiveTo is null OR os2.EffectiveTo > GETDATE()) ) os1 
    join Organisations og1 ON og1.EndPointAssessorOrganisationId = os1.EndPointAssessorOrganisationId
    JOIN OrganisationStandardDeliveryArea od1 ON os1.Id = od1.OrganisationStandardId
    JOIN [DeliveryArea] de1 on de1.Id = od1.DeliveryAreaId
    GROUP BY os1.StandardCode ,TotalEPAOs, ISNULL(Area,'Other'), de1.Ordering
    ) os1 ON sc1.StandardId = os1.StandardCode
    WHERE json_value(StandardData ,'$.IfaStatus') = 'Approved for delivery' AND
    (
        JSON_VALUE(StandardData,'$.EffectiveTo') is null OR
        JSON_VALUE(StandardData,'$.EffectiveTo') > GETDATE() OR
        JSON_VALUE(StandardData,'$.LastDateForNewStarts') is null 
    )

    UNION 

    -- All Regions by All Active Standards
    SELECT * FROM (
    SELECT sc1.StandardId StandardCode, sc1.ReferenceNumber StandardReference, sc1.Title StandardName,JSON_VALUE(sc1.StandardData, '$.Category') Sector, JSON_VALUE(sc1.StandardData, '$.Level') StandardLevel 
    ,ISNULL(Area,'Other') Region, de1.Ordering, 0 Learners, 0 Assessments, 0 TotalEPAOs, 0 EndPointAssessors, NULL EndPointAssessorList
    FROM StandardCollation sc1
    CROSS JOIN [DeliveryArea] de1
    WHERE json_value(StandardData ,'$.IfaStatus') = 'Approved for delivery' AND
    (
        JSON_VALUE(StandardData,'$.EffectiveTo') is null OR
        JSON_VALUE(StandardData,'$.EffectiveTo') > GETDATE() OR
        JSON_VALUE(StandardData,'$.LastDateForNewStarts') is null 
    )
    ) Standards


    UNION

    -- Learners
    SELECT * FROM (
    SELECT StandardCode,  ReferenceNumber StandardReference, Title StandardName, JSON_VALUE(StandardData, '$.Category') Sector, JSON_VALUE(StandardData,'$.Level') StandardLevel,ISNULL(Area,'Other') Region, ISNULL(Ordering,10) Ordering, 0 Learners, COUNT(*) Assessments, 0 TotalEPAOs, 0 EndPointAssessors, NULL EndPointAssessorList
    FROM (
    SELECT ce1.StandardCode,  ISNULL(ISNULL(il1.DelLocPostCode, JSON_VALUE(ce1.Certificatedata,'$.ContactPostCode')),'ZZ99 9ZZ') DelLocPostCode
    FROM [Certificates] ce1
    LEFT JOIN Ilrs il1 ON il1.StdCode = ce1.StandardCode AND il1.Uln = ce1.uln
    WHERE 1=1
    AND IsPrivatelyFunded = 0
    AND ce1.[Status] NOT IN ('Deleted','Draft')
    ) od1
    LEFT JOIN PostCodeRegion pc1 on pc1.PostCodePrefix = LEFT(od1.DelLocPostCode,(PATINDEX('%[0-9]%',ISNULL(od1.DelLocPostCode,'ZZ'))-1))
    LEFT JOIN [DeliveryArea] de1 on de1.Id = pc1.DeliveryAreaId
    JOIN StandardCollation sc1 On sc1.StandardId = od1.StandardCode
    WHERE json_value(StandardData ,'$.IfaStatus') = 'Approved for delivery' AND
    (
        JSON_VALUE(StandardData,'$.EffectiveTo') is null OR
        JSON_VALUE(StandardData,'$.EffectiveTo') > GETDATE() OR
        JSON_VALUE(StandardData,'$.LastDateForNewStarts') is null 
    )
    GROUP BY StandardCode,  ReferenceNumber, Title, JSON_VALUE(StandardData, '$.Category'), JSON_VALUE(StandardData,'$.Level'), ISNULL(Area,'Other'), ISNULL(Ordering,10) 
    ) certs

    ) Total
    WHERE NOT (Region = 'Other' AND Learners = 0)
    GROUP BY StandardCode, StandardReference, StandardName, Sector, StandardLevel, Region,Ordering 
    ORDER BY StandardCode, Ordering;
    
    END TRY
    BEGIN CATCH;
        -- Some basic error handling
        SELECT @Error_Code = ERROR_NUMBER(), @Error_Message = ERROR_MESSAGE();
    END CATCH;
    
    IF @Error_Code <> 0 OR XACT_STATE() = -1 ROLLBACK TRANSACTION T1;
    ELSE IF @Error_Code = 0 AND XACT_STATE() = 1 COMMIT TRANSACTION T1;
END;    