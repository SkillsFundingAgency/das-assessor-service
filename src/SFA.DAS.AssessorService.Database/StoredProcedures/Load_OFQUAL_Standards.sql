CREATE PROCEDURE [dbo].[Load_Ofqual_Standards]
    @dateTimeUtc DATETIME = NULL,
    @inserted INT OUTPUT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION

            IF @dateTimeUtc IS NULL
                SET @dateTimeUtc = GETUTCDATE();

            -------------------------------------------------------------------------------
            -- STEP 1 - Ofqual Organisations 
            -- Move from Ofqual Staging to Ofqual main tables
            -------------------------------------------------------------------------------

            WITH OfqualOrganisationsChanged_CTE AS
            (
                SELECT DISTINCT [RecognitionNumber]
                FROM 
                (
                    SELECT 
                        [RecognitionNumber], [Name], [LegalName], [Acronym], [Email], [Website], [HeadOfficeAddressLine1], [HeadOfficeAddressLine2], [HeadOfficeAddressTown], 
                        [HeadOfficeAddressCounty], [HeadOfficeAddressPostcode], [HeadOfficeAddressCountry], [HeadOfficeAddressTelephone], [OfqualStatus], [OfqualRecognisedFrom], 
                        [OfqualRecognisedTo]
                    FROM [dbo].[OfqualOrganisation]
                    EXCEPT
                    SELECT 
                        [RecognitionNumber], [Name], [LegalName], [Acronym], [Email], [Website], [HeadOfficeAddressLine1], [HeadOfficeAddressLine2], [HeadOfficeAddressTown], 
                        [HeadOfficeAddressCounty], [HeadOfficeAddressPostcode], [HeadOfficeAddressCountry], [HeadOfficeAddressTelephone], [OfqualStatus], [OfqualRecognisedFrom], 
                        [OfqualRecognisedTo]
                    FROM [dbo].[StagingOfqualOrganisation]
            
                    UNION

                    SELECT 
                        [RecognitionNumber], [Name], [LegalName], [Acronym], [Email], [Website], [HeadOfficeAddressLine1], [HeadOfficeAddressLine2], [HeadOfficeAddressTown], 
                        [HeadOfficeAddressCounty], [HeadOfficeAddressPostcode], [HeadOfficeAddressCountry], [HeadOfficeAddressTelephone], [OfqualStatus], [OfqualRecognisedFrom], 
                        [OfqualRecognisedTo]
                    FROM [dbo].[StagingOfqualOrganisation]
                    EXCEPT
                    SELECT 
                        [RecognitionNumber], [Name], [LegalName], [Acronym], [Email], [Website], [HeadOfficeAddressLine1], [HeadOfficeAddressLine2], [HeadOfficeAddressTown], 
                        [HeadOfficeAddressCounty], [HeadOfficeAddressPostcode], [HeadOfficeAddressCountry], [HeadOfficeAddressTelephone], [OfqualStatus], [OfqualRecognisedFrom], 
                        [OfqualRecognisedTo]
                    FROM [dbo].[OfqualOrganisation]
                ) [OrganisationChangesByRecognitionNumber]
            )
            -- merge only where there are new records or changes 
            MERGE INTO [dbo].[OfqualOrganisation] tar
            USING 
            (
                -- providing a default min date for OfqualRecognisedFrom when no value is present in the staging table
                -- this is temporary workaround for non-nullable columns which will be investigated separately
                SELECT
                   soo.[RecognitionNumber]
                  ,[Name]
                  ,[LegalName]
                  ,[Acronym]
                  ,[Email]
                  ,[Website]
                  ,[HeadOfficeAddressLine1]
                  ,[HeadOfficeAddressLine2]
                  ,[HeadOfficeAddressTown]
                  ,[HeadOfficeAddressCounty]
                  ,[HeadOfficeAddressPostcode]
                  ,[HeadOfficeAddressCountry]
                  ,[HeadOfficeAddressTelephone]
                  ,[OfqualStatus]
                  ,ISNULL([OfqualRecognisedFrom], '1900-01-01 00:00:00.000') OfqualRecognisedFrom
                  ,[OfqualRecognisedTo]
                  ,@dateTimeUtc [LastUpdated]
                FROM [dbo].[StagingOfqualOrganisation] soo
                JOIN OfqualOrganisationsChanged_CTE ofoc on ofoc.[RecognitionNumber] = soo.[RecognitionNumber]
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
                    upd.[OfqualRecognisedFrom], upd.[OfqualRecognisedTo], upd.[LastUpdated]);

            -------------------------------------------------------------------------------
            -- STEP 2 Ofqual Qualifications
            -- Move from Ofqual Staging to Ofqual main tables
            -------------------------------------------------------------------------------

            WITH OfqualQualifications_CTE AS
            (
                -- take earliest Operational Start date and the Operational End date from the record with the latest Operational Start date
                -- as when there are mulitple rows for a standard for the same recognition number this is therorized as the EPAO notifying
                -- Ofqual that they are assessing multiple standards, so a later Operational Start date indicates a later version which
                -- should be taken as the Operational End date in preference to earlier ones even when this date would be earlier than others
                SELECT 
                    [RecognitionNumber], [IfateReferenceNumber]
                    ,MAX(CASE WHEN Earliest = 1 THEN [OperationalStartDate] ELSE NULL END) [OperationalStartDate]
                    ,MAX(CASE WHEN Latest = 1 THEN [OperationalEndDate] ELSE NULL END) [OperationalEndDate]
                FROM 
                (
                    SELECT *
                        ,ROW_NUMBER() OVER (PARTITION BY [RecognitionNumber], [IfateReferenceNumber] ORDER BY [OperationalStartDate]) Earliest
                        ,ROW_NUMBER() OVER (PARTITION BY [RecognitionNumber], [IfateReferenceNumber] ORDER BY [OperationalStartDate] DESC, CASE WHEN [OperationalEndDate] IS NULL THEN 0 ELSE 1 END, [OperationalEndDate] DESC) Latest
                    FROM [dbo].[StagingOfqualStandard]
                ) [OperationalDates]
                GROUP BY [RecognitionNumber], [IfateReferenceNumber]
            ),
            OfqualStandardsChanged_CTE AS
            (
                SELECT DISTINCT [RecognitionNumber], [IfateReferenceNumber]
                FROM 
                (
                    SELECT 
                        [RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IfateReferenceNumber]
                    FROM [dbo].[OfqualStandard]
                    EXCEPT
                    SELECT 
                        [RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IfateReferenceNumber]
                    FROM OfqualQualifications_CTE
            
                    UNION
            
                    SELECT 
                        [RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IfateReferenceNumber]
                    FROM OfqualQualifications_CTE
                    EXCEPT
                    SELECT 
                        [RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IfateReferenceNumber]
                    FROM [dbo].[OfqualStandard]
                ) [StandardChangesByRecognitionNumber]
            )
            -- merge only where there are new records or changes 
            MERGE INTO [dbo].[OfqualStandard] tar
            USING 
            (
                SELECT soo.*,  @dateTimeUtc [LastUpdated]  
                FROM
                (
                    SELECT 
                        [RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IfateReferenceNumber] 
                    FROM OfqualQualifications_CTE
                ) soo
                JOIN OfqualStandardsChanged_CTE ofsc on ofsc.[RecognitionNumber] = soo.[RecognitionNumber] AND ofsc.[IfateReferenceNumber] = soo.[IfateReferenceNumber]
            ) upd 
            ON 
            (tar.[RecognitionNumber] = upd.[RecognitionNumber] AND tar.[IfateReferenceNumber] = upd.[IfateReferenceNumber])
            WHEN MATCHED THEN 
            UPDATE SET 
                   tar.[OperationalStartDate] = upd.[OperationalStartDate]
                  ,tar.[OperationalEndDate] = upd.[OperationalEndDate]
                  ,tar.[UpdatedAt] = upd.[LastUpdated]
            WHEN NOT MATCHED BY TARGET THEN
            INSERT ([RecognitionNumber], [OperationalStartDate], [OperationalEndDate], [IfateReferenceNumber], [CreatedAt] )
            VALUES (upd.[RecognitionNumber], upd.[OperationalStartDate], upd.[OperationalEndDate], upd.[IfateReferenceNumber], upd.[LastUpdated] );

            -------------------------------------------------------------------------------
            -- STEP 3
            -- Merge the Ofqual Standards into the EPAO list of Standards
            -------------------------------------------------------------------------------
            -- STEP 3.1
            -- Ofqual Standards List
            -------------------------------------------------------------------------------

            WITH LatestStandardsWithOfqualEqap_CTE
            AS
            (
                SELECT IFateReferenceNumber, LarsCode
                FROM 
                (
                    SELECT 
                        IFateReferenceNumber, LarsCode
                        ,ROW_NUMBER() OVER (PARTITION BY IfateReferenceNumber, LarsCode ORDER BY VersionMajor DESC, VersionMinor DESC) RowNumber 
                    FROM 
                        [dbo].[Standards] 
                    WHERE 
                        [EqaProviderName] = 'Ofqual'
                        AND [Status] IN ('Approved for Delivery', 'Retired')
                        AND VersionApprovedForDelivery IS NOT NULL
                ) [StandardsWithOfqualEqap] WHERE RowNumber = 1
            ),
            OfqualStandards_CTE
            AS
            (
                SELECT o.[Id] [OrganisationId], o.[EndPointAssessorOrganisationId], ofs.[RecognitionNumber], [IfateReferenceNumber], [OperationalStartDate], [OperationalEndDate]
                FROM [dbo].[OfqualStandard] ofs
                JOIN [dbo].[Organisations] o on o.[RecognitionNumber] = ofs.[RecognitionNumber]
            ),
            OrganisationFirstLiveContacts_CTE
            AS
            (
                SELECT [ContactId], [OrganisationId]
                FROM 
                (
                    SELECT 
                        c.[Id] [ContactId], [OrganisationId],  
                        ROW_NUMBER() OVER (PARTITION BY [OrganisationId] ORDER BY c.[CreatedAt] DESC) CreatedAtRowNumber
                    FROM [dbo].[Contacts] c
                        JOIN [Organisations] o ON o.Id = c.OrganisationId
                    WHERE 
                        c.[Status] = 'Live'
                        AND o.[Status] = 'Live'
                ) [FirstLiveContacts] WHERE CreatedAtRowNumber = 1
            ),
            CurrentOfqualStandards_CTE 
            AS
            (
                SELECT DISTINCT o.[RecognitionNumber], os.StandardReference 
                FROM [dbo].[OrganisationStandard] os 
                JOIN [dbo].[Organisations] o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId
                WHERE o.[RecognitionNumber] IS NOT NULL
            )
            
            -- these standards can be added to OrganisationStandard
            INSERT INTO [dbo].[OrganisationStandard]
                ([EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister]
                ,[Comments],[Status],[ContactId],[OrganisationStandardData],[StandardReference])
            SELECT 
                [EndPointAssessorOrganisationId]
                ,[Larscode] [StandardCode]
                ,[OperationalStartDate] [EffectiveFrom]
                ,[OperationalEndDate] [EffectiveTo]
                ,CONVERT(Date, @dateTimeUtc) [DateStandardApprovedOnRegister]
                ,'Added from OFQUAL qualifications list' [Comments]
                ,'Live' [Status]
                ,[ContactId]
                ,'{"DeliveryAreasComments":null}' [OrganisationStandardData]
                ,os.[IfateReferenceNumber] [StandardReference]
            FROM OfqualStandards_CTE os
                JOIN LatestStandardsWithOfqualEqap_CTE lswoe ON lswoe.[IfateReferenceNumber] = os.[IfateReferenceNumber]
                JOIN OrganisationFirstLiveContacts_CTE oflc ON oflc.[OrganisationId] = os.OrganisationId
                LEFT JOIN CurrentOfqualStandards_CTE cofs on cofs.[RecognitionNumber] = os.[RecognitionNumber]
            WHERE 
                cofs.StandardReference IS NULL;

            -- return the number of Standards by EPAO added
            SET @inserted = @@ROWCOUNT;

            -------------------------------------------------------------------------------
            -- STEP 3.2
            -- Add delivery areas for recently added standards
            -------------------------------------------------------------------------------

            WITH AddedOfqualStandardsWithNoDeliveryAreas_CTE
            -- the Ofqual standards added recently, with no delivery areas
            AS
            (
                SELECT [Id] OrganisationStandardId, [Comments]
                FROM [dbo].[OrganisationStandard] os1
                WHERE [Comments] = 'Added from OFQUAL qualifications list'
                AND NOT EXISTS (SELECT null FROM [dbo].[OrganisationStandardDeliveryArea] WHERE [OrganisationStandardId] = os1.[Id])
            )

            -- add all delivery areas 
            INSERT INTO [dbo].[OrganisationStandardDeliveryArea]
                ([OrganisationStandardId],[DeliveryAreaId],[Comments],[Status])
            SELECT 
                [OrganisationStandardId], da.[Id] [DeliveryAreaId], [Comments], 'Live' [Status]
            FROM [AddedOfqualStandardsWithNoDeliveryAreas_CTE]
                CROSS JOIN [dbo].[DeliveryArea] da
            WHERE 
                da.[Status] = 'Live';

            -------------------------------------------------------------------------------
            -- STEP 3.3
            -- Add versions for recently added standards
            -------------------------------------------------------------------------------

            WITH AddedOfqualStandardsWithNoStandardVersions_CTE
            -- the Ofqual standards added recently, with no standard versions
            AS
            (
                SELECT [Id] OrganisationStandardId, [StandardReference], [EffectiveFrom], [DateStandardApprovedOnRegister], [Comments]
                FROM [dbo].[OrganisationStandard] os1
                WHERE [Comments] = 'Added from OFQUAL qualifications list'
                AND NOT EXISTS (SELECT null FROM [dbo].[OrganisationStandardVersion] WHERE [OrganisationStandardId] = os1.[Id])
            ),
            StandardVersionsWithLatestEpaPlan
            -- the version(s) for each standard where the EQAP is "Ofqual" that use the latest EPA Plan
            AS 
            (
                SELECT (CASE WHEN EPAChanges = 0 OR RowNumber <= LatestEPARowNumber THEN 1 ELSE 0 END) AddVersion, * 
                FROM
                (
                    SELECT *  
                        ,MIN(CASE WHEN EPAChanged = 1 THEN RowNumber ELSE NULL END) OVER (PARTITION BY IfateReferenceNumber) LatestEPARowNumber
                        ,SUM(CASE WHEN EPAChanged = 1 THEN 1 ElSE 0 END) OVER (PARTITION BY IfateReferenceNumber) EPAChanges
                    FROM 
                    (
                        SELECT 
                            [IFateReferenceNumber], [Version], [StandardUId], [LarsCode], [VersionEarliestStartDate], [EPAChanged]
                            ,ROW_NUMBER() OVER (PARTITION BY [IfateReferenceNumber] ORDER BY [VersionMajor] DESC, [VersionMinor] DESC) RowNumber 
                        FROM 
                            [dbo].[Standards]
                        WHERE 
                            [EqaProviderName] = 'Ofqual'
                            AND [Status] IN ('Approved for Delivery', 'Retired')
                            AND [VersionApprovedForDelivery] IS NOT NULL
                    ) [OfqualStandardsByVersion] 
                ) [OfqualStandardsByVersionWithLatestEpa]
            )

            -- add versions with latest EPA plan
            INSERT INTO [dbo].[OrganisationStandardVersion] 
                ([StandardUId],[Version],[OrganisationStandardId],[EffectiveFrom],[EffectiveTo],[DateVersionApproved],[Comments],[Status])
            SELECT 
                [StandardUId], [Version], [OrganisationStandardId], 
                (CASE WHEN [VersionEarliestStartDate] > [EffectiveFrom] THEN [VersionEarliestStartDate] ELSE [EffectiveFrom] END) [EffectiveFrom], 
                NULL [EffectiveTo], [DateStandardApprovedOnRegister] [DateVersionApproved], [Comments], 'Live' [Status]
            FROM [StandardVersionsWithLatestEpaPlan] sv
                JOIN [AddedOfqualStandardsWithNoStandardVersions_CTE] aofs on aofs.[StandardReference] = sv.[IfateReferenceNumber]
            WHERE 
                AddVersion = 1;

            -- return the number of standards by EPAO added
            SELECT @inserted

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO