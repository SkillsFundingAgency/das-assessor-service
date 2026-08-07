-- This get the Standard Certficates that Match to a given set of ULNs
-- And then gets 5 alternative Certficiates to act as Masks 
CREATE PROCEDURE [dbo].[Certificates_GetStandardMasks]
    @ExcludeUlns VARCHAR(MAX) = NULL,
    @Top INT = 5
AS
BEGIN
    SET NOCOUNT ON;

DECLARE @CutOffCount int = 20000,
        @CutOffCertificate int,
        @ExcludeUlnsJSON VARCHAR(MAX);

-- this sets limits for the search for performance
SELECT TOP 1 @CutOffCertificate=[CertificateReferenceId] FROM 
(
    SELECT TOP (@CutOffCount) 
        [CertificateReferenceId] 
    FROM [dbo].[StandardCertificateSearchView] 
    ORDER BY [CertificateReferenceId] DESC
) [CutOffCertificates] 
ORDER BY [CertificateReferenceId];

-- ensure that the ULN(s) input is a list of values
SET @ExcludeUlnsJSON = '['+@ExcludeUlns+']';

WITH MatchCerts
AS
(
    SELECT 
        [Uln],
        [CourseCode],
        [Ukprn],
        [Sector]
    FROM [dbo].[StandardCertificateSearchView]
    JOIN OPENJSON(@ExcludeUlnsJSON,'$') ExcludeUlns on ExcludeUlns.[value] = [Uln] 
)

,AllCerts
AS
(
    SELECT 
        ROW_NUMBER() OVER (PARTITION BY [CourseCode] ORDER BY [CreateDay] DESC) Ccseqn,
        ROW_NUMBER() OVER (PARTITION BY [UkPrn] ORDER BY [CreateDay] DESC) Prseqn,
        ROW_NUMBER() OVER (PARTITION BY [Sector] ORDER BY [CreateDay] DESC) SeSeqn,
        [CourseCode],
        [CourseName],
        [CourseLevel],
        [ProviderName],
        [Ukprn],
        [Uln],
        [Sector]
    FROM [dbo].[StandardCertificateSearchView]
    WHERE [CertificateReferenceId] >= @CutOffCertificate  -- for performance
)

SELECT TOP (@Top) 
    'masks' Result,
    'Standard' CertificateType,
    CourseCode,
    CourseName,
    CourseLevel,
    ProviderName
FROM AllCerts a1
WHERE CcSeqn = 1 -- unique Standard
    AND PrSeqn = 1 -- unique Provider
    AND SeSeqn = 1 -- unique Sector
    AND NOT EXISTS 
    (
        SELECT NULL FROM MatchCerts m1 
        WHERE a1.CourseCode = m1.CourseCode
            OR a1.UkPrn = m1.UkPrn
            OR a1.Uln = m1.Uln
            OR a1.Sector = m1.Sector
    )
END
GO
