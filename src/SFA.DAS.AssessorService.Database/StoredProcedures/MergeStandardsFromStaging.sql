CREATE PROCEDURE [dbo].[MergeStandardsFromStaging]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @count1 INT, @count2 INT;

    SELECT @count1 = COUNT(*) FROM [dbo].[StagingStandards];
    SELECT @count2 = COUNT(*) FROM [dbo].[StagingStandardOptions];

    IF @count1 > 0
    BEGIN
        ------------------------------------------------------------------------------------
        -- Standards - insert new standards and update changed existing standards
        -- Standards absent from staging are retained
        -- If the entire standards staging table is empty, this section is skipped
        ------------------------------------------------------------------------------------

        MERGE INTO [dbo].[Standards] stn
        USING [dbo].[StagingStandards] upd
            ON stn.[StandardUId] = upd.[StandardUId]

        WHEN MATCHED AND EXISTS
        (
            SELECT
                stn.[IFateReferenceNumber] COLLATE Latin1_General_100_CS_AS,
                stn.[LarsCode],
                stn.[Title] COLLATE Latin1_General_100_CS_AS,
                stn.[Version],
                stn.[Level],
                stn.[Status] COLLATE Latin1_General_100_CS_AS,
                stn.[TypicalDuration],
                stn.[MaxFunding],
                stn.[IsActive],
                stn.[LastDateStarts],
                stn.[EffectiveFrom],
                stn.[EffectiveTo],
                stn.[VersionEarliestStartDate],
                stn.[VersionLatestStartDate],
                stn.[VersionLatestEndDate],
                stn.[VersionApprovedForDelivery],
                stn.[ProposedTypicalDuration],
                stn.[ProposedMaxFunding],
                stn.[EPAChanged],
                stn.[StandardPageUrl] COLLATE Latin1_General_100_CS_AS,
                stn.[TrailBlazerContact] COLLATE Latin1_General_100_CS_AS,
                stn.[Route] COLLATE Latin1_General_100_CS_AS,
                stn.[VersionMajor],
                stn.[VersionMinor],
                stn.[IntegratedDegree],
                stn.[EqaProviderName] COLLATE Latin1_General_100_CS_AS,
                stn.[EqaProviderContactName] COLLATE Latin1_General_100_CS_AS,
                stn.[EqaProviderContactEmail] COLLATE Latin1_General_100_CS_AS,
                stn.[OverviewOfRole] COLLATE Latin1_General_100_CS_AS,
                stn.[CoronationEmblem],
                stn.[EpaoMustBeApprovedByRegulatorBody]

            EXCEPT

            SELECT
                upd.[IFateReferenceNumber] COLLATE Latin1_General_100_CS_AS,
                upd.[LarsCode],
                upd.[Title] COLLATE Latin1_General_100_CS_AS,
                upd.[Version],
                upd.[Level],
                upd.[Status] COLLATE Latin1_General_100_CS_AS,
                upd.[TypicalDuration],
                upd.[MaxFunding],
                upd.[IsActive],
                upd.[LastDateStarts],
                upd.[EffectiveFrom],
                upd.[EffectiveTo],
                upd.[VersionEarliestStartDate],
                upd.[VersionLatestStartDate],
                upd.[VersionLatestEndDate],
                upd.[VersionApprovedForDelivery],
                upd.[ProposedTypicalDuration],
                upd.[ProposedMaxFunding],
                upd.[EPAChanged],
                upd.[StandardPageUrl] COLLATE Latin1_General_100_CS_AS,
                upd.[TrailBlazerContact] COLLATE Latin1_General_100_CS_AS,
                upd.[Route] COLLATE Latin1_General_100_CS_AS,
                upd.[VersionMajor],
                upd.[VersionMinor],
                upd.[IntegratedDegree],
                upd.[EqaProviderName] COLLATE Latin1_General_100_CS_AS,
                upd.[EqaProviderContactName] COLLATE Latin1_General_100_CS_AS,
                upd.[EqaProviderContactEmail] COLLATE Latin1_General_100_CS_AS,
                upd.[OverviewOfRole] COLLATE Latin1_General_100_CS_AS,
                upd.[CoronationEmblem],
                upd.[EpaoMustBeApprovedByRegulatorBody]
        )
        THEN UPDATE SET
            stn.[IFateReferenceNumber] = upd.[IFateReferenceNumber],
            stn.[LarsCode] = upd.[LarsCode],
            stn.[Title] = upd.[Title],
            stn.[Version] = upd.[Version],
            stn.[Level] = upd.[Level],
            stn.[Status] = upd.[Status],
            stn.[TypicalDuration] = upd.[TypicalDuration],
            stn.[MaxFunding] = upd.[MaxFunding],
            stn.[IsActive] = upd.[IsActive],
            stn.[LastDateStarts] = upd.[LastDateStarts],
            stn.[EffectiveFrom] = upd.[EffectiveFrom],
            stn.[EffectiveTo] = upd.[EffectiveTo],
            stn.[VersionEarliestStartDate] = upd.[VersionEarliestStartDate],
            stn.[VersionLatestStartDate] = upd.[VersionLatestStartDate],
            stn.[VersionLatestEndDate] = upd.[VersionLatestEndDate],
            stn.[VersionApprovedForDelivery] = upd.[VersionApprovedForDelivery],
            stn.[ProposedTypicalDuration] = upd.[ProposedTypicalDuration],
            stn.[ProposedMaxFunding] = upd.[ProposedMaxFunding],
            stn.[EPAChanged] = upd.[EPAChanged],
            stn.[StandardPageUrl] = upd.[StandardPageUrl],
            stn.[TrailBlazerContact] = upd.[TrailBlazerContact],
            stn.[Route] = upd.[Route],
            stn.[VersionMajor] = upd.[VersionMajor],
            stn.[VersionMinor] = upd.[VersionMinor],
            stn.[IntegratedDegree] = upd.[IntegratedDegree],
            stn.[EqaProviderName] = upd.[EqaProviderName],
            stn.[EqaProviderContactName] = upd.[EqaProviderContactName],
            stn.[EqaProviderContactEmail] = upd.[EqaProviderContactEmail],
            stn.[OverviewOfRole] = upd.[OverviewOfRole],
            stn.[CoronationEmblem] = upd.[CoronationEmblem],
            stn.[EpaoMustBeApprovedByRegulatorBody] = upd.[EpaoMustBeApprovedByRegulatorBody],
            stn.[UpdatedAt] = GETDATE()

        WHEN NOT MATCHED
        THEN INSERT
        (
            [StandardUId],
            [IFateReferenceNumber],
            [LarsCode],
            [Title],
            [Version],
            [Level],
            [Status],
            [TypicalDuration],
            [MaxFunding],
            [IsActive],
            [LastDateStarts],
            [EffectiveFrom],
            [EffectiveTo],
            [VersionEarliestStartDate],
            [VersionLatestStartDate],
            [VersionLatestEndDate],
            [VersionApprovedForDelivery],
            [ProposedTypicalDuration],
            [ProposedMaxFunding],
            [EPAChanged],
            [StandardPageUrl],
            [TrailBlazerContact],
            [Route],
            [VersionMajor],
            [VersionMinor],
            [IntegratedDegree],
            [EqaProviderName],
            [EqaProviderContactName],
            [EqaProviderContactEmail],
            [OverviewOfRole],
            [CoronationEmblem],
            [EpaoMustBeApprovedByRegulatorBody],
            [UpdatedAt]
        )
        VALUES
        (
            upd.[StandardUId],
            upd.[IFateReferenceNumber],
            upd.[LarsCode],
            upd.[Title],
            upd.[Version],
            upd.[Level],
            upd.[Status],
            upd.[TypicalDuration],
            upd.[MaxFunding],
            upd.[IsActive],
            upd.[LastDateStarts],
            upd.[EffectiveFrom],
            upd.[EffectiveTo],
            upd.[VersionEarliestStartDate],
            upd.[VersionLatestStartDate],
            upd.[VersionLatestEndDate],
            upd.[VersionApprovedForDelivery],
            upd.[ProposedTypicalDuration],
            upd.[ProposedMaxFunding],
            upd.[EPAChanged],
            upd.[StandardPageUrl],
            upd.[TrailBlazerContact],
            upd.[Route],
            upd.[VersionMajor],
            upd.[VersionMinor],
            upd.[IntegratedDegree],
            upd.[EqaProviderName],
            upd.[EqaProviderContactName],
            upd.[EqaProviderContactEmail],
            upd.[OverviewOfRole],
            upd.[CoronationEmblem],
            upd.[EpaoMustBeApprovedByRegulatorBody],
            GETDATE()
        );
    END;

    ------------------------------------------------------------------------------------
    -- Standard options - replace a standard's complete option list when an incoming
    -- option pair does not already exist in the live table
    --
    -- Removal-only changes do not trigger replacement:
    --   Live A, B; incoming A       -> live A, B are retained
    --
    -- An incoming new option does trigger replacement:
    --   Live A, B; incoming A, C    -> live options become A, C; B is removed
    --
    -- Standards with no staged option rows keep their existing options
    -- If the entire options staging table is empty, this section is skipped
    ------------------------------------------------------------------------------------

    IF @count2 > 0
    BEGIN
        -- Find standard IDs with at least one incoming option pair absent from live.
        -- Delete ALL live options for those standards, ready to reload their
        -- complete staged lists in the following INSERT.
        DELETE FROM [dbo].[StandardOptions]
        WHERE [StandardUId] IN
        (
            SELECT DISTINCT [StandardUId]
            FROM
            (
                SELECT [StandardUId], [OptionName]
                FROM [dbo].[StagingStandardOptions]

                EXCEPT

                SELECT [StandardUId], [OptionName]
                FROM [dbo].[StandardOptions]
            ) changedOptions
        );

        -- Insert complete staged option lists for standard IDs that now have
        -- no live options. This includes IDs cleared by the DELETE above and
        -- incoming IDs that did not previously have any live options.
        -- IDs that still have live options are left unchanged.
        INSERT INTO [dbo].[StandardOptions]
        (
            [StandardUId],
            [OptionName]
        )
        SELECT
            sso.[StandardUId],
            sso.[OptionName]
        FROM [dbo].[StagingStandardOptions] sso
        INNER JOIN
        (
            SELECT DISTINCT [StandardUId]
            FROM [dbo].[StagingStandardOptions]

            EXCEPT

            SELECT DISTINCT [StandardUId]
            FROM [dbo].[StandardOptions]
        ) standardsWithoutOptions
            ON standardsWithoutOptions.[StandardUId] = sso.[StandardUId];
    END;
END;