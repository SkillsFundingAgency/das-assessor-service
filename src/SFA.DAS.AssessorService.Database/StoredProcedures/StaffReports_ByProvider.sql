CREATE PROCEDURE [dbo].[StaffReports_ByProvider]
AS
  SELECT 
	  CASE ce.[ProviderUkPrn] 
			WHEN 10004344 THEN 'MIDDLESBROUGH COLLEGE'
			WHEN 10005991 THEN 'CENTRAL COLLEGE NOTTINGHAM'
			WHEN 10007315 THEN 'WALSALL COLLEGE'
			WHEN 10007949 THEN 'HUNTINGDONSHIRE REGIONAL COLLEGE'
			WHEN 10021221 THEN 'MERSEY CARE NHS FOUNDATION TRUST'
			WHEN 10001548 THEN 'COLLEGE OF HARINGEY, ENFIELD AND NORTH-EAST LONDON, THE'
			WHEN 10007144 THEN 'UNIVERSITY OF EAST LONDON'
			WHEN 10005404 THEN 'REASEHEATH COLLEGE'
			WHEN 10005077 THEN 'PETERBOROUGH REGIONAL COLLEGE'
			WHEN 10007431 THEN 'WEST SUFFOLK COLLEGE'
			WHEN 10007697 THEN 'YH TRAINING SERVICES LIMITED'
			WHEN 10022210 THEN 'ANNE CLARKE ASSOCIATES LIMITED'
			WHEN 10053962 THEN 'NEWCASTLE COLLEGE'
			ELSE
			(CASE WHEN ce.[ProviderName] IS NULL OR ce.[ProviderName] = 'UNKNOWN' THEN 'UKPRN '+ CONVERT(VARCHAR, ce.[ProviderUkPrn]) ELSE ce.[ProviderName] END)
		END AS 'Provider Name', 
	  COUNT(*) AS 'Total',
	  SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS 'Manual Total',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS 'EPA Service Total',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[Createdby] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Service',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[Createdby] = 'manual' THEN 1 ELSE 0 END ) AS 'EPA Service (Manual)',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS 'EPA Draft',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS 'EPA Submitted',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS 'EPA Printed',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted'
  FROM 
	(
		SELECT a1.*,
		(SELECT MAX(UPPER(JSON_VALUE([CertificateData], '$.ProviderName'))) FROM [dbo].[Certificates] WHERE [ProviderUkPrn] = a1.[ProviderUkPrn]) [ProviderName]
		FROM  [dbo].[Certificates] a1
	) ce
  GROUP BY 
	CASE ce.[ProviderUkPrn] 
		WHEN 10004344 THEN 'MIDDLESBROUGH COLLEGE'
		WHEN 10005991 THEN 'CENTRAL COLLEGE NOTTINGHAM'
		WHEN 10007315 THEN 'WALSALL COLLEGE'
		WHEN 10007949 THEN 'HUNTINGDONSHIRE REGIONAL COLLEGE'
		WHEN 10021221 THEN 'MERSEY CARE NHS FOUNDATION TRUST'
		WHEN 10001548 THEN 'COLLEGE OF HARINGEY, ENFIELD AND NORTH-EAST LONDON, THE'
		WHEN 10007144 THEN 'UNIVERSITY OF EAST LONDON'
		WHEN 10005404 THEN 'REASEHEATH COLLEGE'
		WHEN 10005077 THEN 'PETERBOROUGH REGIONAL COLLEGE'
		WHEN 10007431 THEN 'WEST SUFFOLK COLLEGE'
		WHEN 10007697 THEN 'YH TRAINING SERVICES LIMITED'
		WHEN 10022210 THEN 'ANNE CLARKE ASSOCIATES LIMITED'
		WHEN 10053962 THEN 'NEWCASTLE COLLEGE'
		ELSE
		(CASE WHEN ce.[ProviderName] IS NULL OR ce.[ProviderName] = 'UNKNOWN' THEN 'UKPRN '+ CONVERT(VARCHAR, ce.[ProviderUkPrn]) ELSE ce.[ProviderName] END)
	END

  UNION

  SELECT
	  ' Summary' AS 'Provider Name', 
	  COUNT(*) AS 'Total',
	  SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS 'Manual Total',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS 'EPA Service Total',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[Createdby] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Service',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[Createdby] = 'manual' THEN 1 ELSE 0 END ) AS 'EPA Service (Manual)',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS 'EPA Draft',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS 'EPA Submitted',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS 'EPA Printed',
	  SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted'
  FROM [dbo].[Certificates] ce

  ORDER BY 5 DESC, 4 DESC, 2 DESC, 1
RETURN 0
