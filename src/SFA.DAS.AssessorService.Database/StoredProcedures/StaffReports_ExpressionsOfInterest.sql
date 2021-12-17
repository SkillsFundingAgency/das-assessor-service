-- Commented out as Expression of interest as it has been temporarily removed and would require rework if readded

--CREATE PROCEDURE [StaffReports_ExpressionsOfInterest]

--AS

--SELECT
--	ISNULL(sc1.[Title], sc2.[Title]) "Name of standard",
--	StandardReference "Standard reference",
--	CASE WHEN sc1.ReferenceNumber IS NOT NULL THEN JSON_VALUE(sc1.[StandardData],'$.Level') ELSE JSON_VALUE(sc2.[StandardData],'$.Level') END "Standard level",
--	[Email] "Email address",
--	[OrganisationName] "Organisation name",
--	ISNULL(ContactName,'') "Contact name",
--	ISNULL(ContactPhone,'') "Contact number",
--	CONVERT(VARCHAR(10),CreatedAt) SubmissionDate
--FROM [ExpressionsOfInterest] eo1
--LEFT JOIN [StandardCollation] sc1 ON sc1.ReferenceNumber = eo1.StandardReference
--LEFT JOIN [StandardNonApprovedCollation] sc2 ON sc2.ReferenceNumber = eo1.StandardReference
--WHERE eo1.Status = 'New'
--ORDER BY CreatedAt DESC;

--RETURN 0