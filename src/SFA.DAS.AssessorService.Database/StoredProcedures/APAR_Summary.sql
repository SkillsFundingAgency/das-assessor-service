CREATE PROCEDURE [dbo].[APAR_Summary]
AS
BEGIN
    DECLARE
        @updated int = 0;

    WITH APARdata
    AS (
    SELECT
      os.EndPointAssessorOrganisationId, EndPointAssessorName, EndPointAssessorUkprn,
      MIN(os.DateStandardApprovedOnRegister) EarliestDateStandardApprovedOnRegister,
      MIN(os.EffectiveFrom) EarliestEffectiveFromDate
    FROM
      OrganisationStandard os
      INNER JOIN 
      (
        SELECT OrganisationStandardId, StandardUId 
        FROM OrganisationStandardVersion 
        WHERE (EffectiveTo IS NULL OR EffectiveTo > GETDATE()) 
        AND [Status] = 'Live' 
      ) [ActiveStandardVersions] ON [ActiveStandardVersions].OrganisationStandardId = os.Id
      INNER JOIN Organisations o ON os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId 
      INNER JOIN 
      (
        SELECT StandardUId
        FROM Standards 
        WHERE Larscode != 0 
        AND (EffectiveTo IS NULL OR EffectiveTo > GETDATE())
      ) [ActiveStandards] ON [ActiveStandards].StandardUid = [ActiveStandardVersions].StandardUId
      WHERE
        o.[Status] = 'Live'
        AND o.EndPointAssessorOrganisationId <> 'EPA0000'
        AND (os.EffectiveTo IS NULL OR os.EffectiveTo > GETDATE())
        AND os.[Status] = 'Live'
        AND o.[EndPointAssessorUkprn] IS NOT NULL
        AND o.[EndPointAssessorUkprn] < 90000000
      GROUP BY 
        os.EndPointAssessorOrganisationId, EndPointAssessorName, EndPointAssessorUkprn
    )
    -- Update, Add or Remove EPAOs
    MERGE INTO [dbo].[APARSummary] apar
    USING APARdata ad1
    ON (apar.EndPointAssessorOrganisationId = ad1.EndPointAssessorOrganisationId)
    WHEN MATCHED AND (
        apar.[EndPointAssessorName] != ad1.[EndPointAssessorName] OR
        apar.[EndPointAssessorUkprn] != ad1.[EndPointAssessorUkprn] OR
        apar.[EarliestDateStandardApprovedOnRegister] != ad1.[EarliestDateStandardApprovedOnRegister] OR
        apar.[EarliestEffectiveFromDate] != ad1.[EarliestEffectiveFromDate] )
    THEN 
        UPDATE SET 
        apar.[EndPointAssessorName] = ad1.[EndPointAssessorName],
        apar.[EndPointAssessorUkprn] = ad1.[EndPointAssessorUkprn],
        apar.[EarliestDateStandardApprovedOnRegister] = ad1.[EarliestDateStandardApprovedOnRegister],
        apar.[EarliestEffectiveFromDate] = ad1.[EarliestEffectiveFromDate]
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT VALUES (ad1.[EndPointAssessorOrganisationId],ad1.[EndPointAssessorName],ad1.[EndPointAssessorUkprn],ad1.[EarliestDateStandardApprovedOnRegister],ad1.[EarliestEffectiveFromDate])
    WHEN NOT MATCHED BY SOURCE THEN 
        DELETE;

    -- return the number of rows changed by Merge
    SET @updated = @@ROWCOUNT;  

    IF @updated > 0 
    BEGIN
    -- record the date&time of the latest changes
        UPDATE [dbo].[APARSummaryUpdated] SET [LastUpdated] = GETUTCDATE();
        IF @@ROWCOUNT = 0
        INSERT INTO [dbo].[APARSummaryUpdated] ([LastUpdated]) VALUES ( GETUTCDATE() );
    END
        
    -- return the number of changes made to APAR for EPAOs
    SELECT @updated

END
GO
