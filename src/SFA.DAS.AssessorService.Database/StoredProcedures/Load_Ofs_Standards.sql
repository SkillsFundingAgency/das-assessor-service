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
                        AND [HighestLevelOfDegreeAwardingPowers] <> 'Not applicable'
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

            -- return the number of Standards by EPAO added
            SET @inserted = @@ROWCOUNT;

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