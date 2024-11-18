CREATE PROCEDURE [dbo].[PopulateLearner_WithdrawOverlappingIlrsAndLearners]
    @TestMode BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CASE 
            -- Rule 0: Certificate exists
            WHEN CertificateId IS NOT NULL
                THEN 0 -- good
            -- Rule 1: Most recent
            WHEN Next_LearnStartDate IS NULL
                THEN 1 -- good
            -- Rule 2: LearnStart is after Next End (allowing a couple of months overlap for EPA)
            WHEN DATEDIFF(month,EOMONTH([Next_EndDate]),EOMONTH([LearnStartDate])) > -2
                THEN 2 -- good
            -- Rule 3: Unspecified End (should not happen, but there are some)
            WHEN EndDate IS NULL 
                THEN 3  -- bad
            -- Rule 4: Next LearnStart is after End (allowing a couple of months overlap for EPA)
            WHEN DATEDIFF(month,EOMONTH([EndDate]), EOMONTH(Next_LearnStartDate)) > -2
                THEN 4 -- good
            -- Rule 5: Replacement as Next LearnStart same as LearnStart
            WHEN EOMONTH([LearnStartDate]) = EOMONTH([Next_LearnStartDate])
                THEN 5  -- bad
            -- Rule 6: LearnStart is before Next End, (Next LearnStart is NOT after End in Rule 4)
            WHEN DATEDIFF(month, EOMONTH([Next_LearnStartDate]), EOMONTH([LearnStartDate])) >= 0
                AND DATEDIFF(month, EOMONTH([LearnStartDate]), EOMONTH([Next_EndDate])) >= 0
                THEN 6  -- bad
            -- Rule 7: Next LearnStart before End, (LearnStart is NOT after Next End in Rule 2)
            WHEN DATEDIFF(month, EOMONTH([LearnStartDate]), EOMONTH([Next_LearnStartDate])) >= 0
                AND DATEDIFF(month, EOMONTH([Next_LearnStartDate]), EOMONTH([EndDate])) >= 0
                THEN 7  -- bad
            ELSE 8  -- should not happen
        END AS PossibleOverlap,
        [IlrsDuplicatedByUlnWithNextRecord].[Id], 
        [IlrsDuplicatedByUlnWithNextRecord].[CompletionStatus]
    INTO #IlrsDuplicatedByUlnWithNextRecord
    FROM
    (
        SELECT 
            [Ilrs].[Id],
            [Certificates].Id AS CertificateId,
            LEAD([Ilrs].[UkPrn]) OVER (PARTITION BY [Ilrs].[Uln] ORDER BY [Source], [Ilrs].[CreatedAt], [LearnStartDate]) AS Next_Ukprn,
            CONVERT(DATE, [LearnStartDate]) AS [LearnStartDate],
            CONVERT(DATE, LEAD([LearnStartDate]) OVER (PARTITION BY [Ilrs].[Uln] ORDER BY [Source], [Ilrs].[CreatedAt], [LearnStartDate])) AS Next_LearnStartDate,
            CONVERT(DATE, ISNULL([LearnActEndDate], [PlannedEndDate])) AS EndDate,
            CONVERT(DATE, ISNULL(LEAD([LearnActEndDate]) OVER (PARTITION BY [Ilrs].[Uln] ORDER BY [LastUpdated]), 
                                 LEAD([PlannedEndDate]) OVER (PARTITION BY [Ilrs].[Uln] ORDER BY [Source], [Ilrs].[CreatedAt], [LearnStartDate]))) AS Next_EndDate,
            [CompletionStatus],
            CONVERT(DATE, LEAD([LastUpdated]) OVER (PARTITION BY [Ilrs].[Uln] ORDER BY [Source], [Ilrs].[CreatedAt], [LearnStartDate])) AS Next_LastUpdated
        FROM 
            [Ilrs]
        JOIN 
        (
            SELECT 
                [Uln]
            FROM 
                [dbo].[Ilrs]
            WHERE 
                [Uln] NOT IN (1000000000, 9999999999)
            GROUP BY 
                [Uln]
            HAVING 
                COUNT(*) > 1
        ) AS [IlrsDuplicatedByUln] ON [IlrsDuplicatedByUln].Uln = [Ilrs].Uln
        LEFT JOIN [Certificates] ON [Certificates].[Uln] = [Ilrs].[Uln] AND [Certificates].[StandardCode] = [Ilrs].[StdCode]
    ) AS [IlrsDuplicatedByUlnWithNextRecord]
    
    SELECT [Overlapping].[Id] 
    INTO 
        #IlrIdsToBeWithdrawn
    FROM 
        #IlrsDuplicatedByUlnWithNextRecord [Overlapping]
    WHERE 
        [Overlapping].PossibleOverlap IN ( 5, 6, 7 )
        AND Overlapping.CompletionStatus NOT IN ( 3, 6 );

    -- withdraw the Learner records
    MERGE INTO [dbo].[Learner] [Target]
    USING #IlrIdsToBeWithdrawn [Source]
    ON ([Target].[Id] = [Source].[Id])
    WHEN MATCHED THEN UPDATE SET [CompletionStatus] = 3;

    -- withdraw the Ilrs
    MERGE INTO [dbo].[Ilrs] [Target]
    USING #IlrIdsToBeWithdrawn [Source]
    ON ([Target].[Id] = [Source].[Id])
    WHEN MATCHED THEN UPDATE SET [CompletionStatus] = 3;
    
    IF @TestMode = 1
    BEGIN
        SELECT PossibleOverlap, Id, CompletionStatus
        FROM #IlrsDuplicatedByUlnWithNextRecord [Overlapping]
    END
END
GO