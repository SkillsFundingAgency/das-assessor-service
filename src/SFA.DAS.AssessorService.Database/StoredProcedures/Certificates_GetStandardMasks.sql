-- This get the Standard Certficates that Match to a given set of ULNs
-- And then gets 5 alternative Certficiates to act as Masks 
CREATE PROCEDURE [dbo].[Certificates_GetStandardMasks]
    @ExcludeUlns VARCHAR(MAX) = NULL,
    @Top INT = 5
AS
BEGIN
    SET NOCOUNT ON;

DECLARE @CutoffDay date;
-- this sets limits for the search for performance
SELECT @CutoffDay = dateadd(month,-6,MAX([CreateDay])) FROM [dbo].[Certificates];

WITH MatchCerts
AS
(
SELECT ce1.[Id]
      ,ce1.[Uln]
      ,ce1.[StandardCode]
	  ,ce1.[StandardName] 
	  ,CONVERT(varchar,ce1.[StandardLevel]) CourseLevel
      ,ce1.[ProviderUkPrn] Ukprn
	  ,ce1.[ProviderName]
      ,st1.[Route] Sector
  FROM [dbo].[StandardCertificates] ce1
  JOIN [dbo].[Standards] st1 on st1.[StandardUId] = ce1.[StandardUId]
  WHERE 1=1
  AND ce1.[Status] NOT IN ('draft','deleted')
  AND (
			@ExcludeUlns IS NOT NULL
			AND ce1.[ULN] IN
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
SELECT CONVERT(char(10),ce1.[CreateDay],121) CreatedDate
      ,ROW_NUMBER() OVER (PARTITION BY ce1.[StandardCode] ORDER BY ce1.[CreateDay] DESC) Stseqn
      ,ROW_NUMBER() OVER (PARTITION BY ce1.[ProviderUkPrn] ORDER BY ce1.[CreateDay] DESC) Prseqn
      ,ROW_NUMBER() OVER (PARTITION BY ce1.[LearningStartDate] ORDER BY ce1.[CreateDay] DESC) LsSeqn
      ,CONVERT(varchar,ce1.[StandardCode]) CourseCode
      ,ce1.[StandardName] CourseName
      ,CONVERT(varchar,ce1.[StandardLevel]) CourseLevel
      ,ce1.[ProviderName]
  FROM [dbo].[StandardCertificates] ce1
  JOIN [dbo].[Standards] st1 on st1.[StandardUId] = ce1.[StandardUId]
  WHERE 1=1
  AND ce1.[Status] NOT IN ('draft','deleted')
  AND ce1.[ULN] > 10000000     -- there are invalid ULNs (these should be fixed if possible)
  AND ce1.[CreateDay] >= @CutoffDay  -- for performance
  AND NOT EXISTS (
	  SELECT NULL FROM MatchCerts m1 
	  WHERE 1=1
	  AND ce1.Id = m1.Id
	  AND ISNULL(ce1.StandardCode, -1) = ISNULL(m1.StandardCode, -1)
	  AND ISNULL(ce1.ProviderUkPrn, -1) = ISNULL(m1.UkPrn, -1)
      AND ISNULL(ce1.Uln, '') = ISNULL(m1.Uln, '')
      AND ISNULL(st1.[Route], '') = ISNULL(m1.Sector, '')
  )
)

SELECT TOP (@Top) 'masks' Result
      ,'Standard' CertificateType
      ,CourseCode
      ,CourseName
      ,CourseLevel
      ,ProviderName
FROM AllCerts a1
WHERE 1=1 
AND StSeqn = 1 -- unique Standard
AND PrSeqn <= 2 -- unique Provider
AND LsSeqn <= 3 -- unique Start

END
GO


