CREATE PROCEDURE [dbo].[PopulateLearner_WithdrawSupersededIlrs]
AS
    -- Withdrawals are not being done in a timely manner by training providers. When the course code has changed the 
    -- training provider should submit an ILR for the previous course code marking it as withdrawn; when they don't
    -- do this we get duplicated ApprenticeshipId records in the ILR import from Data Collection and duplicated learners
    -- in the Learner table. We will receive the current course code from Approvals after it has imported the updated ILR
    -- and the data lock has been processed, this can be used to identify which learners have been duplicated and which
    -- ILR's should be marked as withdrawn
    UPDATE [dbo].[Ilrs]
    SET 
        [CompletionStatus] = 3, -- (3) Withdrawn
        [UpdatedAt] = GETUTCDATE()
    WHERE [Id] IN 
    (
        SELECT 
            [Id] 
        FROM [dbo].[Learner] l INNER JOIN [dbo].[ApprovalsExtract] ae 
        ON ae.[ApprenticeshipId] = l.[ApprenticeshipId] AND ae.[TrainingCode] <> l.[StdCode]
    )

RETURN 0
