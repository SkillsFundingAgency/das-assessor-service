/*
Post-Deployment Script Template                            
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.        
 Use SQLCMD syntax to include a file in the post-deployment script.            
 Example:      :r .\myfile.sql                                
 Use SQLCMD syntax to reference a variable in the post-deployment script.        
 Example:      :setvar TableName MyTable                            
               SELECT * FROM [$(TableName)]                    
--------------------------------------------------------------------------------------
*/


-- ON-613 Patch Certificates with STxxxx StandardReference, where it is not yet included. 
-- AB 11/03/19 Keep this active for new deployments, for now
-- AB 31/07/19 Still seeing existance of certs without Standard reference (need to understand why)
-- ****************************************************************************
MERGE INTO certificates ma1
USING (
SELECT ce1.[Id],JSON_MODIFY([CertificateData],'$.StandardReference',st1.ReferenceNumber) newData
  FROM [Certificates] ce1 
  JOIN [StandardCollation] st1 ON ce1.StandardCode = st1.StandardId
  WHERE st1.ReferenceNumber IS NOT NULL 
  AND JSON_VALUE([CertificateData],'$.StandardReference') IS NULL) up1
ON (ma1.id = up1.id)
WHEN MATCHED THEN UPDATE SET ma1.[CertificateData] = up1.[newData];


-- START OF ON-1952
UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'HEI' AND FinancialExempt = 0
UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'Public Sector' AND FinancialExempt = 0
UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'College' AND FinancialExempt = 0 
UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'Academy or Free School' AND FinancialExempt = 0
-- END OF ON-1952

-- START OF ON-2089
IF NOT EXISTS (SELECT * FROM EMailTemplates WHERE TemplateName = N'ApplyEPAOResponse')
BEGIN
INSERT [dbo].[EmailTemplates] ([Id], [TemplateName], [TemplateId], [Recipients], [RecipientTemplate], [CreatedAt],[UpdatedAt],  [DeletedAt]) 
VALUES (N'eb20ee3c-516e-4e44-97ea-3fd8f70039ef',  N'ApplyEPAOResponse', N'84174eab-f3c1-4274-8670-2fb5b21cbd77', N'epao.helpdesk@education.gov.uk', NULL, CAST(N'2019-01-08T11:52:09.2030000' AS DateTime2), NULL, NULL)
END
IF NOT EXISTS (SELECT * FROM EMailTemplates WHERE TemplateName = N'ApplyEPAOUpdate')
BEGIN
INSERT [dbo].[EmailTemplates] ([Id],  [TemplateName], [TemplateId], [Recipients], [RecipientTemplate], [CreatedAt],  [UpdatedAt],  [DeletedAt]) 
VALUES (N'bca6b89f-6d77-47c7-87e9-439628ada40a', N'ApplyEPAOUpdate', N'ffe63c0d-b2b0-461f-b99a-73105d7d5fa3', N'epao.helpdesk@education.gov.uk', NULL, CAST(N'2019-01-08T11:52:09.2170000' AS DateTime2), NULL, NULL)
END
IF NOT EXISTS (SELECT * FROM EMailTemplates WHERE TemplateName = N'ApplyEPAOInitialSubmission')
BEGIN
INSERT [dbo].[EmailTemplates] ([Id],  [TemplateName], [TemplateId], [Recipients], [RecipientTemplate], [CreatedAt],  [UpdatedAt],  [DeletedAt]) 
VALUES (N'b66dfd61-5cc3-4a0b-83a2-84f63f3e3371',  N'ApplyEPAOInitialSubmission', N'68410850-909b-4669-a60a-f60e4b1cb89f', N'epao.helpdesk@education.gov.uk', NULL, CAST(N'2019-01-08T11:52:09.2400000' AS DateTime2),  NULL, NULL)
END
IF NOT EXISTS (SELECT * FROM EMailTemplates WHERE TemplateName = N'ApplySignupError')
BEGIN
INSERT [dbo].[EmailTemplates] ([Id], [TemplateName], [TemplateId], [Recipients], [RecipientTemplate], [CreatedAt],  [UpdatedAt], [DeletedAt]) 
VALUES (N'01dd414e-585c-47cf-8c89-ba1b84cfb103',  N'ApplySignupError', N'88799189-fe12-4887-a13f-f7f76cd6945a', N'epao.helpdesk@education.gov.uk', NULL, CAST(N'2019-01-08T11:52:09.2200000' AS DateTime2),  NULL, NULL)
END
IF NOT EXISTS (SELECT * FROM EMailTemplates WHERE TemplateName = N'ApplyEPAOStandardSubmission')
BEGIN
INSERT [dbo].[EmailTemplates] ([Id],  [TemplateName], [TemplateId], [Recipients], [RecipientTemplate], [CreatedAt],  [UpdatedAt], [DeletedAt]) 
VALUES (N'a701f4a4-2672-4da9-8005-e6eef1025963',  N'ApplyEPAOStandardSubmission', N'e0a52c44-10be-4164-9543-3c312769c4e3', N'epao.helpdesk@education.gov.uk', NULL, CAST(N'2019-01-08T11:52:09.2430000' AS DateTime2), NULL, NULL)
END
-- END OF ON-2089

-- ON-2197 - PostCode to Region Mapping 
:r .\Insert-Postcode-to-Regions.sql