CREATE PROCEDURE [dbo].[PrepareStandardsImport]
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DELETE FROM [dbo].[StagingStandardOptions];
    DELETE FROM [dbo].[StagingStandards];
END;
