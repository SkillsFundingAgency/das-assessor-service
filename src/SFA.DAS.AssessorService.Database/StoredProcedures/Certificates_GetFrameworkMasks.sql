-- This gets the Framework Certficates that Match to a given set of ULNs
-- And then gets 5 alternative Certficiates to act as Masks

CREATE PROCEDURE [dbo].[Certificates_GetFrameworkMasks]
    @ExcludeUlns VARCHAR(MAX) = NULL,
    @Top INT = 5
AS
BEGIN
    SET NOCOUNT ON;

DECLARE @CutoffDay date,
        @ExcludeUlnsJSON VARCHAR(4000);

-- this is to prevent a full scan of all history to get masks
SET @CutoffDay = '2021-01-01';

-- ensure that the ULN(s) input are a list of values
SET @ExcludeUlnsJSON = '['+@ExcludeUlns+']';

WITH MatchCerts
AS
(
    SELECT 
        [Uln],
        [CourseCode],
        [ProviderName]
    FROM [dbo].[FrameworkCertificateSearchView]
    JOIN OPENJSON(@ExcludeUlnsJSON,'$') ExcludedUlns on ExcludedUlns.[value] = [Uln] 
)

,AllCerts
AS
(
    SELECT 
        ROW_NUMBER() OVER (PARTITION BY [CourseCode] ORDER BY [CreateDay] DESC) Ccseqn,
        ROW_NUMBER() OVER (PARTITION BY [ProviderName] ORDER BY [CreateDay] DESC) Prseqn,
        ROW_NUMBER() OVER (PARTITION BY [DateAwarded] ORDER BY [CreateDay] DESC) DaSeqn,
        CourseCode,
        CourseName,
        CourseLevel,
        [ProviderName],
        ISNULL([Uln],0) Uln
  FROM [dbo].[FrameworkCertificateSearchView] fe1
  WHERE [CreateDay] > @CutoffDay 
    AND Ukprn IS NOT NULL 
)

SELECT TOP (@Top) 
    'masks' Result,
    'Framework' CertificateType,
    CourseCode,
    CourseName,
    CourseLevel,
    ProviderName
FROM AllCerts ac
WHERE CcSeqn = 1
    AND PrSeqn = 1
    AND DaSeqn = 1
    AND NOT EXISTS 
    (
        SELECT NULL FROM MatchCerts mc 
        WHERE ac.[Uln] = mc.[Uln]
            OR ac.CourseCode = mc.CourseCode
            OR ac.ProviderName = mc.ProviderName
    )
END
GO
