-- This gets the Framework Certficates that Match to a given set of ULNs
-- And then gets 5 alternative Certficiates to act as Masks

CREATE PROCEDURE [dbo].[Certificates_GetFrameworkMasks]
    @ExcludeUlns VARCHAR(MAX) = NULL,
    @Top INT = 5
AS
BEGIN
    SET NOCOUNT ON;

DECLARE @CutoffDay date;
-- this is to prevent a full scan of all history to get masks
SET @CutoffDay = '2021-01-01';

WITH MatchCerts
AS
(
SELECT fl1.[Id]
      ,fl1.[ApprenticeULN] [Uln]
      ,fl1.[TrainingCode] 
	  ,fl1.[FrameworkName]
      ,fl1.[ApprenticeshipLevel] 
      ,fl1.[ProviderName]
   FROM [dbo].[FrameworkLearner] fl1
  WHERE 1=1
  AND (
			@ExcludeUlns IS NULL
			OR fl1.[ApprenticeULN] IN
                (
                    SELECT TRY_CAST(value AS BIGINT)
                    FROM STRING_SPLIT(@ExcludeUlns, ',')
                    WHERE TRY_CAST(value AS BIGINT) IS NOT NULL
                )
	  )
)
,AllCerts
AS
(
SELECT CONVERT(char(10),fl1.[CreatedOn],121) CreatedDate
      ,ROW_NUMBER() OVER (PARTITION BY fl1.[PathwayName] ORDER BY fl1.createdOn DESC) Tcseqn
      ,ROW_NUMBER() OVER (PARTITION BY fl1.[ProviderName] ORDER BY fl1.createdOn DESC) Prseqn
      ,ROW_NUMBER() OVER (PARTITION BY fl1.[ApprenticeStartDate] ORDER BY fl1.createdOn DESC) StSeqn
      ,fl1.[TrainingCode] CourseCode
      ,fl1.[FrameworkName] CourseName
      ,fl1.[ApprenticeshipLevel] CourseLevel
      ,fl1.[ProviderName]
  FROM [dbo].[FrameworkLearner] fl1
  WHERE 1=1
  AND fl1.[CreatedOn] > @CutoffDay 
  AND fl1.Ukprn IS NOT NULL 
  AND NOT EXISTS (
	  SELECT NULL FROM MatchCerts m1 WHERE 1=1
	  AND fl1.Id = m1.Id
	  AND fl1.[ApprenticeUln] = m1.[Uln]
	  AND fl1.TrainingCode = m1.TrainingCode
	  AND fl1.ProviderName = m1.ProviderName
  )
)

SELECT TOP (@Top) 'masks' Result
      ,'Framework' CertificateType
	  ,null [ULN]
      ,CourseCode
      ,CourseName
      ,CourseLevel
      ,ProviderName
FROM AllCerts a1
WHERE 1=1 
AND TcSeqn = 1
AND PrSeqn <= 2
AND StSeqn <= 3
END
GO


