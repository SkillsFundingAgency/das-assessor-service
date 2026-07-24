-- This gets the Framework Certficates that Match to a given set of ULNs
-- And then gets 5 alternative Certficiates to act as Masks

CREATE PROCEDURE [dbo].[Certificates_GetFrameworkMasks]
    @ExcludeUlns VARCHAR(MAX) = NULL,
    @Top INT = 5
AS
BEGIN
    SET NOCOUNT ON;

DECLARE @CutoffDay date,
        @UlnJSON VARCHAR(4000);

-- this is to prevent a full scan of all history to get masks
SET @CutoffDay = '2021-01-01';

-- ensure that the ULN(s) input are a list of values
SET @UlnJSON = '['+@ExcludeUlns+']';

WITH MatchCerts
AS
(
SELECT [Uln]
      ,[CourseCode] 
      ,[ProviderName]
  FROM [dbo].[FrameworkCertificateSearchView]
  JOIN OPENJSON(@UlnJSON,'$') ulns on ulns.[value] = [Uln] 
)

,AllCerts
AS
(
SELECT ROW_NUMBER() OVER (PARTITION BY [CourseCode] ORDER BY [CreateDay] DESC) Tcseqn
      ,ROW_NUMBER() OVER (PARTITION BY [ProviderName] ORDER BY [CreateDay] DESC) Prseqn
      ,ROW_NUMBER() OVER (PARTITION BY [DateAwarded] ORDER BY [CreateDay] DESC) DaSeqn
      ,CourseCode
      ,CourseName
      ,CourseLevel
      ,[ProviderName]
      ,ISNULL([Uln],0) Uln
  FROM [dbo].[FrameworkCertificateSearchView] fe1
  WHERE 1=1
  AND [CreateDay] > @CutoffDay 
  AND Ukprn IS NOT NULL 

)

SELECT TOP (@Top) 'masks' Result
      ,'Framework' CertificateType
      ,CourseCode
      ,CourseName
      ,CourseLevel
      ,ProviderName
FROM AllCerts a1
WHERE 1=1 
AND TcSeqn = 1
AND PrSeqn = 1
AND DaSeqn = 1
AND NOT EXISTS (
    SELECT NULL FROM MatchCerts m1 
    WHERE 1=0
    OR a1.[Uln] = m1.[Uln]
    OR a1.CourseCode = m1.CourseCode
    OR a1.ProviderName = m1.ProviderName
  )
END
GO
