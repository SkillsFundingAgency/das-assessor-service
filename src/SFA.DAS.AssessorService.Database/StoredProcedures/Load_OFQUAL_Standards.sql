CREATE PROCEDURE [dbo].[Load_OFQUAL_Standards]
AS
BEGIN
    DECLARE
        @inserted int = 0;
        
-------------------------------------------------------------------------------
-- STEP 1 - OFQUAL Organisations 
-- Move from OFQUAL Staging to OFQUAL main tables
-------------------------------------------------------------------------------

    WITH 
    OFQUAL_Changed AS
    (
    SELECT DISTINCT [RecognitionNumber]
    FROM (
    SELECT [RecognitionNumber], [Name], [LegalName], [Acronym], [Email], [Website], [HeadOfficeAddressLine1], [HeadOfficeAddressLine2], [HeadOfficeAddressTown], 
    [HeadOfficeAddressCounty], [HeadOfficeAddressPostcode], [HeadOfficeAddressCountry], [HeadOfficeAddressTelephone], [OfqualStatus], [OfqualRecognisedFrom], 
    [OfqualRecognisedTo]
    FROM [dbo].[OfqualOrganisation]
    EXCEPT
    SELECT [RecognitionNumber], [Name], [LegalName], [Acronym], [Email], [Website], [HeadOfficeAddressLine1], [HeadOfficeAddressLine2], [HeadOfficeAddressTown], 
    [HeadOfficeAddressCounty], [HeadOfficeAddressPostcode], [HeadOfficeAddressCountry], [HeadOfficeAddressTelephone], [OfqualStatus], [OfqualRecognisedFrom], 
    [OfqualRecognisedTo]
    FROM [dbo].[StagingOfqualOrganisation]
    UNION
    SELECT [RecognitionNumber], [Name], [LegalName], [Acronym], [Email], [Website], [HeadOfficeAddressLine1], [HeadOfficeAddressLine2], [HeadOfficeAddressTown], 
    [HeadOfficeAddressCounty], [HeadOfficeAddressPostcode], [HeadOfficeAddressCountry], [HeadOfficeAddressTelephone], [OfqualStatus], [OfqualRecognisedFrom], 
    [OfqualRecognisedTo]
    FROM [dbo].[StagingOfqualOrganisation]
    EXCEPT
    SELECT [RecognitionNumber], [Name], [LegalName], [Acronym], [Email], [Website], [HeadOfficeAddressLine1], [HeadOfficeAddressLine2], [HeadOfficeAddressTown], 
    [HeadOfficeAddressCounty], [HeadOfficeAddressPostcode], [HeadOfficeAddressCountry], [HeadOfficeAddressTelephone], [OfqualStatus], [OfqualRecognisedFrom], 
    [OfqualRecognisedTo]
    FROM [dbo].[OfqualOrganisation]
    ) rn1
    )
    -- merge only where there are new records or changes 
    MERGE INTO [dbo].[OfqualOrganisation] tar
    USING 
    (
    SELECT soo.*,  GETUTCDATE() [LastUpdated]  
    FROM [dbo].[StagingOfqualOrganisation] soo
    JOIN OFQUAL_Changed ofc on ofc.[RecognitionNumber] = soo.[RecognitionNumber]
    ) upd 
    ON 
    (tar.[RecognitionNumber] = upd.[RecognitionNumber])
    WHEN MATCHED THEN 
    UPDATE SET 
           tar.[Name] = upd.[Name]
          ,tar.[LegalName] = upd.[LegalName]
          ,tar.[Acronym] = upd. [Acronym]
          ,tar.[Email] = upd.[Email]
          ,tar.[Website] = upd.[Website]
          ,tar.[HeadOfficeAddressLine1] = upd.[HeadOfficeAddressLine1]
          ,tar.[HeadOfficeAddressLine2] = upd.[HeadOfficeAddressLine2]
          ,tar.[HeadOfficeAddressTown] = upd.[HeadOfficeAddressTown]
          ,tar.[HeadOfficeAddressCounty] = upd.[HeadOfficeAddressCounty]
          ,tar.[HeadOfficeAddressPostcode] = upd.[HeadOfficeAddressPostcode]
          ,tar.[HeadOfficeAddressCountry] = upd.[HeadOfficeAddressCountry]
          ,tar.[HeadOfficeAddressTelephone] = upd.[HeadOfficeAddressTelephone]
          ,tar.[OfqualStatus] = upd.[OfqualStatus]
          ,tar.[OfqualRecognisedFrom] = upd.[OfqualRecognisedFrom]
          ,tar.[OfqualRecognisedTo] = upd.[OfqualRecognisedTo]
          ,tar.[UpdatedAt] = upd.[LastUpdated]
    WHEN NOT MATCHED BY TARGET THEN
    INSERT ([RecognitionNumber], [Name], [LegalName], [Acronym], [Email], [Website], 
            [HeadOfficeAddressLine1], [HeadOfficeAddressLine2], [HeadOfficeAddressTown], [HeadOfficeAddressCounty], 
            [HeadOfficeAddressPostcode], [HeadOfficeAddressCountry],  [HeadOfficeAddressTelephone], 
            [OfqualStatus], [OfqualRecognisedFrom], [OfqualRecognisedTo], [CreatedAt] )
    VALUES (upd.[RecognitionNumber], upd.[Name], upd.[LegalName], upd.[Acronym], upd.[Email], upd.[Website], 
            upd.[HeadOfficeAddressLine1], upd.[HeadOfficeAddressLine2], upd.[HeadOfficeAddressTown], upd.[HeadOfficeAddressCounty], 
            upd.[HeadOfficeAddressPostcode], upd.[HeadOfficeAddressCountry], upd.[HeadOfficeAddressTelephone], upd.[OfqualStatus], 
            upd.[OfqualRecognisedFrom], upd.[OfqualRecognisedTo], upd.[LastUpdated] )
    ;

    -------------------------------------------------------------------------------
    -- STEP 2 OFQUAL Qualifications
    -- Move from OFQUAL Staging to OFQUAL main tables
    -------------------------------------------------------------------------------

    WITH OFQUAL_Qualifications AS
    (
    -- take earliest Operational Start date and the Operational End date from the record with the latest Operational Start date
    SELECT [RecognitionNumber], [IFateReferenceNumber]
    ,MAX(CASE WHEN Earliest = 1 THEN [OperationalStartDate] ELSE NULL END) [OperationalStartDate]
    ,MAX(CASE WHEN Latest = 1 THEN [OperationalEndDate] ELSE NULL END) [OperationalEndDate]
    FROM (
    SELECT *
    ,ROW_NUMBER() OVER (PARTITION BY [RecognitionNumber], [IFateReferenceNumber] ORDER BY [OperationalStartDate]) Earliest
    ,ROW_NUMBER() OVER (PARTITION BY [RecognitionNumber], [IFateReferenceNumber] ORDER BY [OperationalStartDate] DESC,CASE WHEN [OperationalEndDate] IS NULL THEN 0 ELSE 1 END,[OperationalEndDate] DESC) Latest
    FROM [dbo].[StagingOfqualStandard]
    ) ab1
    GROUP BY [RecognitionNumber], [IFateReferenceNumber]
    )
    ,
    OFQUAL_Changed AS
    (
    SELECT DISTINCT [RecognitionNumber], [IFateReferenceNumber]
    FROM (
    SELECT [RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IFateReferenceNumber]
    FROM [dbo].[OfqualStandard]
    EXCEPT
    SELECT [RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IFateReferenceNumber]
    FROM OFQUAL_Qualifications
    UNION
    SELECT [RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IFateReferenceNumber]
    FROM OFQUAL_Qualifications
    EXCEPT
    SELECT [RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IFateReferenceNumber]
    FROM [dbo].[OfqualStandard]
    ) rn1
    )
    -- merge only where there are new records or changes 
    MERGE INTO [dbo].[OfqualStandard] tar
    USING 
    (
    SELECT soo.*,  GETUTCDATE() [LastUpdated]  
    FROM (
        SELECT [RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IFateReferenceNumber] 
        FROM OFQUAL_Qualifications
    ) soo
    JOIN OFQUAL_Changed ofc on ofc.[RecognitionNumber] = soo.[RecognitionNumber] AND ofc.[IFateReferenceNumber] = soo.[IFateReferenceNumber]
    ) upd 
    ON 
    (tar.[RecognitionNumber] = upd.[RecognitionNumber] AND tar.[IFateReferenceNumber] = upd.[IFateReferenceNumber])
    WHEN MATCHED THEN 
    UPDATE SET 
           tar.[OperationalStartDate] = upd.[OperationalStartDate]
          ,tar.[OperationalEndDate] = upd.[OperationalEndDate]
          ,tar.[UpdatedAt] = upd.[LastUpdated]
    WHEN NOT MATCHED BY TARGET THEN
    INSERT ([RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IFateReferenceNumber], [CreatedAt] )
    VALUES (upd.[RecognitionNumber], upd.[OperationalStartDate], upd.[OperationalEndDate], upd.[IFateReferenceNumber], upd.[LastUpdated] )
    ;



-------------------------------------------------------------------------------
-- STEP 3
-- Merge the OFQUAL Standards into the EPAO list of Standards
-------------------------------------------------------------------------------
-- STEP 3.1
-- OFQUAL Standards List
-------------------------------------------------------------------------------

    WITH 
    OFQUAL_Standards
    -- The Standards / Versions that have the EQAP as "ofqual"
    AS
    (
    SELECT IFateReferenceNumber,  Larscode
    FROM (
        SELECT IFateReferenceNumber, Title, Version, Level, StandardUId, Larscode, ProposedTypicalDuration Duration, 
               ROW_NUMBER() OVER (PARTITION BY IFateReferenceNumber, LarsCode ORDER BY VersionMajor DESC, VersionMinor DESC) rownumber 
        FROM [dbo].[Standards] 
        WHERE [EqaProviderName] = 'Ofqual'
        AND [Status] IN ('Approved for Delivery', 'Retired')
        AND VersionApprovedForDelivery IS NOT NULL
    ) sv1 WHERE rownumber = 1
    ),
    OFQUAL_Qualifications
    AS
    (
    SELECT org.[Id] [OrganisationId], org.[EndPointAssessorOrganisationId], sos.[RecognitionNumber], [IFateReferenceNumber], [OperationalStartDate], [OperationalEndDate]
    FROM [dbo].[OfqualStandard] sos
    JOIN [dbo].[Organisations] org on org.[RecognitionNumber] = sos.[RecognitionNumber]
    ),
    Organisation_Contacts
    AS
    (
    SELECT * FROM (
        SELECT co1.[Id] [ContactId], [OrganisationId],  
        ROW_NUMBER() OVER (PARTITION BY [OrganisationId] ORDER BY co1.[CreatedAt] DESC) seq1
        FROM [dbo].[Contacts] co1
        JOIN [Organisations] og1 ON og1.Id = co1.OrganisationId
        WHERE co1.[Status] = 'Live'
        AND og1.[Status] = 'Live'
    ) ab1 WHERE seq1 = 1
    ),
    All_EPAO_Standards 
    AS
    (
    SELECT DISTINCT og1.[RecognitionNumber], os1.StandardReference 
    FROM [dbo].[OrganisationStandard] os1 
    JOIN [dbo].[Organisations] og1 on os1.EndPointAssessorOrganisationId = og1.EndPointAssessorOrganisationId
    WHERE og1.[RecognitionNumber] IS NOT NULL
    )
    -- These Standards can be added to OrganisationStandard
    INSERT INTO [dbo].[OrganisationStandard]
    ([EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister]
    ,[Comments],[Status],[ContactId],[OrganisationStandardData],[StandardReference])
    -- 
    SELECT 
        [EndPointAssessorOrganisationId]
        ,[Larscode] [StandardCode]
        ,[OperationalStartDate] [EffectiveFrom]
        ,[OperationalEndDate] [EffectiveTo]
        ,CONVERT(Date,GETUTCDATE()) [DateStandardApprovedOnRegister]
        ,'Added from OFQUAL qualifications list' [Comments]
        ,'Live' [Status]
        ,[ContactId]
        ,'{"DeliveryAreasComments":null}' [OrganisationStandardData]
        ,ofq.[IFateReferenceNumber] [StandardReference]
    FROM OFQUAL_Qualifications ofq
    JOIN OFQUAL_Standards ofs ON ofs.[IFateReferenceNumber] = ofq.[IFateReferenceNumber]
    JOIN Organisation_Contacts ocs ON ocs.[OrganisationId] = ofq.OrganisationId
    LEFT JOIN All_EPAO_Standards aes on aes.[RecognitionNumber] = ofq.[RecognitionNumber]
    WHERE aes.StandardReference IS NULL;

    -- return the number of Standards by EPAO added
    SET @inserted = @@ROWCOUNT;    

-------------------------------------------------------------------------------
-- STEP 3.2
-- Add delivery areas for recently added Standards
-------------------------------------------------------------------------------

    WITH AddedOFQUALStandards
    -- The OFQUAL Standards Added recently, with no delivery area
    AS
    (
    SELECT [Id] OrganisationStandardId, [Comments]
    FROM [dbo].[OrganisationStandard] os1
    WHERE [Comments] = 'Added from OFQUAL qualifications list'
    AND NOT EXISTS (SELECT null FROM [dbo].[OrganisationStandardDeliveryArea] WHERE [OrganisationStandardId] = os1.[Id])
    )
    -- add Delivery Areas 
    INSERT INTO [dbo].[OrganisationStandardDeliveryArea]
    ([OrganisationStandardId],[DeliveryAreaId],[Comments],[Status])
    SELECT [OrganisationStandardId], dea.[Id] [DeliveryAreaId], [Comments], 'Live' [Status]
    FROM AddedOFQUALStandards ads 
    CROSS JOIN [dbo].[DeliveryArea] dea
    WHERE dea.[Status] = 'Live';

-------------------------------------------------------------------------------
-- STEP 3.3
-- -- Add versions for recently added Standards
-------------------------------------------------------------------------------

    WITH AddedOFQUALStandards
    -- The OFQUAL Standards Added recently
    AS
    (
    SELECT [Id] OrganisationStandardId, [StandardReference], [EffectiveFrom], [DateStandardApprovedOnRegister], [Comments]
    FROM [dbo].[OrganisationStandard] os1
    WHERE [Comments] = 'Added from OFQUAL qualifications list'
    AND NOT EXISTS (SELECT null FROM [dbo].[OrganisationStandardVersion] WHERE [OrganisationStandardId] = os1.[Id])
    ),
    StandardVersions
    -- the latest version(s) for each Standard where the EQA Provider is "ofqual" that use the latest EPA Plan
    AS (
    SELECT (CASE WHEN EPAChanges = 0 OR rownumber <= LatestEPA THEN 1 ELSE 0 END) AddVersion, * 
    FROM
    (
        SELECT *  
        ,MAX(CASE WHEN EPAChanged = 1 THEN rownumber ELSE 0 END) OVER (PARTITION BY IfateReferenceNumber) LatestEPA
        ,SUM(CASE WHEN EPAChanged = 1 THEN 1 ElSE 0 END) OVER (PARTITION BY IfateReferenceNumber) EPAChanges
        FROM (
        SELECT [IFateReferenceNumber], [Version], [StandardUId], [Larscode], [VersionEarliestStartDate], [EPAChanged],
            ROW_NUMBER() OVER (PARTITION BY [IFateReferenceNumber] ORDER BY [VersionMajor] DESC, [VersionMinor] DESC) rownumber 
        FROM [dbo].[Standards]
        WHERE [EqaProviderName] = 'Ofqual'
        AND [Status] IN ('Approved for Delivery', 'Retired')
        AND [VersionApprovedForDelivery] IS NOT NULL
        ) ab1 
    ) ab2
    WHERE 1=1 OR rownumber <= LatestEPA
    )
    -- add versions with Latest EPA Plan
    INSERT INTO [dbo].[OrganisationStandardVersion] 
    ([StandardUId],[Version],[OrganisationStandardId],[EffectiveFrom],[EffectiveTo],[DateVersionApproved],[Comments],[Status])
    SELECT [StandardUId], [Version], [OrganisationStandardId], 
    (CASE WHEN [VersionEarliestStartDate] > [EffectiveFrom] THEN [VersionEarliestStartDate] ELSE [EffectiveFrom] END) [EffectiveFrom], 
    NULL [EffectiveTo], [DateStandardApprovedOnRegister] [DateVersionApproved], [Comments], 'Live' [Status]
    FROM StandardVersions stv
    JOIN AddedOFQUALStandards ads on ads.[StandardReference] = stv.[IFateReferenceNumber]
    WHERE AddVersion = 1;

    
    -- return the number of Standards by EPAO added
    SELECT @inserted
END;
GO