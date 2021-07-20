CREATE PROCEDURE [dbo].[StaffReports_ByStandard]
AS
select 
st.[Standard Name], 
st.[Standard Reference], 
STRING_AGG(st.[Standard Version], ', ') WITHIN GROUP (ORDER BY CONVERT(decimal(5,2), st.[Standard Version]) ASC) [Standard Version], 
SUM(st.Total) [Total], 
SUM(st.[Manual Total]) [Manual Total], 
SUM(st.[EPA Service Total]) [EPA Service Total], 
SUM(st.[EPA Service]) [EPA Service], 
SUM(st.[EPA Service Manual]) [EPA Service Manual], 
SUM(st.[EPA Draft]) [EPA Draft], 
SUM(st.[EPA Submitted]) [EPA Submitted], 
SUM(st.[EPA Printed]) [EPA Printed], 
SUM(st.Deleted) [Deleted]
from
(

SELECT
			REPLACE(UPPER(REPLACE(JSON_VALUE(ce.[CertificateData], '$.StandardName'), NCHAR(0x00A0), ' ')), 'Á', ' ') AS 'Standard Name',
			JSON_VALUE(ce.CertificateData, '$.StandardReference') 'Standard Reference',
			JSON_VALUE(ce.CertificateData, '$.Version') 'Standard Version',
			COUNT(*) AS 'Total',
			SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS 'Manual Total',
			SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS 'EPA Service Total',
			SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Service',
			SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END ) AS 'EPA Service Manual',
			SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS 'EPA Draft',
			SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS 'EPA Submitted',
			SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS 'EPA Printed',
			SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted'
	  FROM [dbo].[Certificates] ce
	  GROUP BY REPLACE(UPPER(REPLACE(JSON_VALUE(ce.[CertificateData], '$.StandardName'), NCHAR(0x00A0), ' ')), 'Á', ' '), ce.[StandardCode],  ce.CertificateData
)
as st
group by st.[Standard Name], st.[Standard Reference]



  UNION 
  SELECT
		' Summary' AS 'Standard Name',
		''  AS 'Standard Reference',
		''  AS 'Standard Version',
		COUNT(*) AS 'Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] < 10000 THEN 1 ELSE 0 END) AS 'Manual Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 THEN 1 ELSE 0 END) AS 'EPA Service Total',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] <> 'manual' THEN 1 ELSE 0 END) AS 'EPA Service',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 And ce.[CreatedBy] = 'manual' THEN 1 ELSE 0 END ) AS 'EPA Service Manual',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Draft' THEN 1 ELSE 0 END) AS 'EPA Draft',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Submitted' THEN 1 ELSE 0 END) AS 'EPA Submitted',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NULL AND ce.[Status] = 'Printed' THEN 1 ELSE 0 END) AS 'EPA Printed',
		SUM(CASE WHEN ce.[CertificateReferenceId] >= 10000 AND ce.[DeletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS 'Deleted'
  FROM [dbo].[Certificates] ce

  ORDER BY 6 DESC, 5 DESC, 3 DESC, 1
RETURN 0