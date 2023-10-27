CREATE PROCEDURE [dbo].[Load_Ofs_Standards]
    @dateTimeUtc DATETIME = NULL,
    @inserted INT OUTPUT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION

            IF @dateTimeUtc IS NULL
                SET @dateTimeUtc = GETUTCDATE();

            -------------------------------------------------------------------------------
            -- STEP 1 - Ofs Organisations 
            -- Move from Ofs Staging to Ofs main tables
            -------------------------------------------------------------------------------

            WITH OfsOrganisationsChanged_CTE AS
            (
                SELECT DISTINCT [Ukprn]
                FROM 
                (
                    SELECT 
                        [Ukprn]
                    FROM 
                        [dbo].[StagingOfsOrganisation]
                    WHERE
                        [RegistrationStatus] = 'Registered'
                        AND ([HighestLevelOfDegreeAwardingPowers] <> 'Not applicable' OR [HighestLevelOfDegreeAwardingPowers] IS NULL)
                    EXCEPT
                    SELECT 
                        [Ukprn]
                    FROM 
                        [dbo].[OfsOrganisation]
                ) [OrganisationChangesByUkprn]
            )
            -- merge only where there are new records or changes 
            MERGE INTO [dbo].[OfsOrganisation] tar
            USING 
            (
                SELECT
                   soo.[Ukprn]
                  ,@dateTimeUtc [LastUpdated]
                FROM [dbo].[StagingOfsOrganisation] soo
                JOIN OfsOrganisationsChanged_CTE ofoc on ofoc.[Ukprn] = soo.[Ukprn]
            ) upd 
            ON 
            (tar.[Ukprn] = upd.[Ukprn])
            WHEN NOT MATCHED BY TARGET THEN
            INSERT ([Ukprn], [CreatedAt] )
            VALUES (upd.[Ukprn], upd.[LastUpdated]);

            -------------------------------------------------------------------------------
            -- STEP 3
            -- Merge the Ofs Standards into the EPAO list of Standards
            -------------------------------------------------------------------------------
            -- STEP 3.1
            -- Ofs Standards List
            -------------------------------------------------------------------------------

            DECLARE @academicYear VARCHAR(4) = (SELECT 
            CASE 
                WHEN MONTH(@dateTimeUtc) < 8 THEN 
                    RIGHT(CONVERT(VARCHAR, YEAR(@dateTimeUtc) - 1), 2) + RIGHT(CONVERT(VARCHAR, YEAR(@dateTimeUtc)), 2)
                ELSE 
                    RIGHT(CONVERT(VARCHAR, YEAR(@dateTimeUtc)), 2) + RIGHT(CONVERT(VARCHAR, YEAR(@dateTimeUtc) + 1), 2)
            END);

            WITH LatestStandardsWithOfsEqap_CTE
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
                        [EqaProviderName] = 'Office for Students'
                        AND [Status] IN ('Approved for Delivery', 'Retired')
                        AND VersionApprovedForDelivery IS NOT NULL
                        AND EpaoMustBeApprovedByRegulatorBody = 0
                ) [StandardsWithOfqualEqap] WHERE RowNumber = 1
            ),
            IlrStandards_CTE
            AS
            (
                SELECT 
                    DISTINCT UkPrn, StdCode 
                FROM 
                    [dbo].[Ilrs] 
                WHERE 
                    [Source] >= @academicYear
                    AND [CompletionStatus] in (1 , 2)
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
            CurrentOfsStandards_CTE 
            AS
            (
                SELECT DISTINCT o.EndPointAssessorUkprn, os.StandardReference 
                FROM [dbo].[OrganisationStandard] os 
                JOIN [dbo].[Organisations] o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId
                WHERE o.EndPointAssessorUkprn IS NOT NULL
            )
            
            -- these standards can be added to OrganisationStandard
            INSERT INTO [dbo].[OrganisationStandard]
                ([EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister]
                ,[Comments],[Status],[ContactId],[OrganisationStandardData],[StandardReference])
            SELECT 
                [EndPointAssessorOrganisationId]
                ,[Larscode] [StandardCode]
                ,CONVERT(Date, @dateTimeUtc) [EffectiveFrom]
                ,null [EffectiveTo]
                ,CONVERT(Date, @dateTimeUtc) [DateStandardApprovedOnRegister]
                ,'Added from OFS matching ILR data' [Comments]
                ,'Live' [Status]
                ,[ContactId]
                ,'{"DeliveryAreasComments":null}' [OrganisationStandardData]
                ,lswoe.[IfateReferenceNumber] [StandardReference]
            FROM IlrStandards_CTE ist
                JOIN [dbo].[Providers] p on p.Ukprn = ist.UkPrn
                JOIN [dbo].OfsOrganisation oo on oo.Ukprn = p.Ukprn
                JOIN [dbo].Organisations o on o.EndPointAssessorUkprn = oo.Ukprn
                JOIN OrganisationFirstLiveContacts_CTE oflc ON oflc.[OrganisationId] = o.Id
                JOIN LatestStandardsWithOfsEqap_CTE lswoe ON lswoe.LarsCode = ist.StdCode
                LEFT JOIN CurrentOfsStandards_CTE cofs on cofs.[EndPointAssessorUkprn] = o.[EndPointAssessorUkprn] 
                AND cofs.[StandardReference] = lswoe.[IfateReferenceNumber]
            WHERE 
                cofs.StandardReference IS NULL;

            -- return the number of Standards by EPAO added
            SET @inserted = @@ROWCOUNT;

            -------------------------------------------------------------------------------
            -- STEP 3.2
            -- Add delivery areas for recently added standards
            -------------------------------------------------------------------------------

            WITH AddedOfsStandardsWithNoDeliveryAreas_CTE
            -- the Ofs standards added recently, with no delivery areas
            AS
            (
                SELECT [Id] OrganisationStandardId, [Comments]
                FROM [dbo].[OrganisationStandard] os1
                WHERE [Comments] = 'Added from OFS matching ILR data'
                AND NOT EXISTS (SELECT null FROM [dbo].[OrganisationStandardDeliveryArea] WHERE [OrganisationStandardId] = os1.[Id])
            )

            -- add all delivery areas 
            INSERT INTO [dbo].[OrganisationStandardDeliveryArea]
                ([OrganisationStandardId],[DeliveryAreaId],[Comments],[Status])
            SELECT 
                [OrganisationStandardId], da.[Id] [DeliveryAreaId], [Comments], 'Live' [Status]
            FROM [AddedOfsStandardsWithNoDeliveryAreas_CTE]
                CROSS JOIN [dbo].[DeliveryArea] da
            WHERE 
                da.[Status] = 'Live';

            -------------------------------------------------------------------------------
            -- STEP 3.3
            -- Add versions for recently added standards
            -------------------------------------------------------------------------------

            WITH AddedOfsStandardsWithNoStandardVersions_CTE
            -- the Ofs standards added recently, with no standard versions
            AS
            (
                SELECT [Id] OrganisationStandardId, [StandardReference], [EffectiveFrom], [DateStandardApprovedOnRegister], [Comments]
                FROM [dbo].[OrganisationStandard] os1
                WHERE [Comments] = 'Added from OFS matching ILR data'
                AND NOT EXISTS (SELECT null FROM [dbo].[OrganisationStandardVersion] WHERE [OrganisationStandardId] = os1.[Id])
            ),
            StandardVersionsWithLatestEpaPlan
            -- the version(s) for each standard where the EQAP is "Ofs" that use the latest EPA Plan
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
                            [EqaProviderName] = 'Office for Students'
                            AND [Status] IN ('Approved for Delivery', 'Retired')
                            AND VersionApprovedForDelivery IS NOT NULL
                            AND EpaoMustBeApprovedByRegulatorBody = 0
                    ) [OfsStandardsByVersion] 
                ) [OfsStandardsByVersionWithLatestEpa]
            )

            -- add versions with latest EPA plan
            INSERT INTO [dbo].[OrganisationStandardVersion] 
                ([StandardUId],[Version],[OrganisationStandardId],[EffectiveFrom],[EffectiveTo],[DateVersionApproved],[Comments],[Status])
            SELECT 
                [StandardUId], [Version], [OrganisationStandardId], 
                (CASE WHEN [VersionEarliestStartDate] > [EffectiveFrom] THEN [VersionEarliestStartDate] ELSE [EffectiveFrom] END) [EffectiveFrom], 
                NULL [EffectiveTo], [DateStandardApprovedOnRegister] [DateVersionApproved], [Comments], 'Live' [Status]
            FROM [StandardVersionsWithLatestEpaPlan] sv
                JOIN [AddedOfsStandardsWithNoStandardVersions_CTE] aofs on aofs.[StandardReference] = sv.[IfateReferenceNumber]
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