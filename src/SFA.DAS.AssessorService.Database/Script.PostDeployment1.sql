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

-- backup ILRS before data synch
/* DONE
DELETE FROM IlrsCopy

INSERT INTO IlrsCopy SELECT * FROM Ilrs
*/

/* DONE
update deliveryarea set Ordering=1 where Area='North East'
update deliveryarea set Ordering=2 where Area='North West'
update deliveryarea set Ordering=3 where Area='Yorkshire and the Humber'
update deliveryarea set Ordering=4 where Area='East Midlands'
update deliveryarea set Ordering=5 where Area='West Midlands'
update deliveryarea set Ordering=6 where Area='East of England'
update deliveryarea set Ordering=7 where Area='London'
update deliveryarea set Ordering=8 where Area='South East'
update deliveryarea set Ordering=9 where Area='South West*/

-- ON-1374 update any new organisation standards to 'Live' if minimum acceptance criteria for live is available
UPDATE organisationStandard 
    SET Status='Live', 
    DateStandardApprovedOnRegister = ISNULL(DateStandardApprovedOnRegister, CONVERT(DATE, GETDATE()))
    WHERE Id IN (SELECT organisationStandardId FROM  OrganisationStandardDeliveryArea)
    AND contactId IS NOT NULL
    AND Status='New'

/* DONE
-- ON-1058 update FHA details STORY 
:r UpdateFHADetails.sql
*/

/* DONE
-- load December 2018 report DATABASE
:r setDec18EPAReport.sql
*/

-- patch FundingModel, where this was not set by data sync
UPDATE Ilrs SET FundingModel = 36 WHERE FundingModel IS NULL

/* DONE
-- fix options
UPDATE [Certificates]
SET [CertificateData] = JSON_MODIFY([CertificateData], '$.CourseOption','Alcoholic Beverage Service') 
WHERE json_value(certificatedata,'$.CourseOption') = 'Alcholic beverage service'

UPDATE [Options] 
SET [OptionName] = 'Alcoholic Beverage Service'
WHERE [OptionName] = 'Alcholic beverage service'
*/

-- ON-613 Patch Certificates with STxxxx StandardReference, where it is not yet included. 
-- AB 11/03/19 Keep this active for new deployments, for now
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

IF NOT EXISTS (SELECT * FROM EMailTemplates WHERE TemplateName = N'EPAOUserApproveConfirm')
BEGIN
INSERT EMailTemplates ([Id],[TemplateName],[TemplateId],[Recipients],[CreatedAt]) 
VALUES (NEWID(), N'EPAOUserApproveConfirm', N'539204f8-e99a-4efa-9d1f-d0e58b26dd7b', NULL, GETDATE())
END

IF NOT EXISTS (SELECT * FROM EMailTemplates WHERE TemplateName = N'EPAOUserApproveRequest')
BEGIN
INSERT EMailTemplates ([Id],[TemplateName],[TemplateId],[Recipients],[CreatedAt]) 
VALUES (NEWID(), N'EPAOUserApproveRequest', N'5bb920f4-06ec-43c7-b00a-8fad33ce8066', NULL, GETDATE())
END

-- setup Privileges
IF NOT EXISTS (SELECT * FROM [Privileges] WHERE [UserPrivilege] = N'Manage users')
BEGIN
INSERT [Privileges] ([Id],[UserPrivilege]) VALUES (NEWID(), N'Manage users')
END

IF NOT EXISTS (SELECT * FROM [Privileges] WHERE [UserPrivilege] =  N'Record grades and issue certificates')
BEGIN
INSERT [Privileges] ([Id],[UserPrivilege]) VALUES (NEWID(), N'Record grades and issue certificates')
END

IF NOT EXISTS (SELECT * FROM [Privileges] WHERE [UserPrivilege] =  N'View standards')
BEGIN
INSERT [Privileges] ([Id],[UserPrivilege]) VALUES (NEWID(), N'View standards')
END

IF NOT EXISTS (SELECT * FROM [Privileges] WHERE [UserPrivilege] =  N'Apply for standards')
BEGIN
INSERT [Privileges] ([Id],[UserPrivilege]) VALUES (NEWID(), N'Apply for standards')
END

-- Setup ContactsPrivileges
delete from [ContactsPrivileges]

insert into [ContactsPrivileges]
select co1.id, pr1.id 
from Contacts co1 
cross  join [Privileges] pr1
where co1.status = 'Live'  and co1.username not like 'unknown%' and co1.username != 'manual'

--Setup contact roles
INSERT INTO [ContactRoles]
SELECT ab1.*, co1.id contactid FROM (
SELECT newid() Id,'SuperUser' Rolename ) ab1
CROSS JOIN [Contacts] co1
WHERE co1.[Status] = 'Live'
AND co1.username not like 'unknown%'
AND EXISTS ( SELECT NULL FROM Organisations og1 WHERE og1.id = co1.OrganisationId AND og1.[Status] != 'Deleted')
AND NOT EXISTS (SELECT NULL FROM [ContactRoles] co2 WHERE co2.ContactId = co1.Id)

-- DisplayName fix
MERGE INTO [Contacts] co1
USING (
SELECT T.id,
    T.DisplayName, T.Email,
	TRIM(CASE WHEN SecondSpace.j = 0 OR SUBSTRING(T.DisplayName,1,FirstSpace.i) = 'null' THEN '' ELSE SUBSTRING(T.DisplayName,1,FirstSpace.i) END) Title,
    ISNULL(TRIM(GivenNames.g),'') "GivenNames", ISNULL(TRIM(FamilyName.f),'') "FamilyName"
FROM (
SELECT 
CASE WHEN [Id] = '9A60D39C-19D8-48E2-AE10-08D5D52AA3F6' THEN 'null Agnes Varadi'
     WHEN [Id] = 'A671441F-A6C9-458E-A68C-08D64A4EB5A1' THEN 'null Emma Tune'
     WHEN [Id] = 'A8B2F30B-EE5B-438E-F621-08D5DB4F0956' THEN 'null Mark Thomas'
     WHEN [Id] = 'BACEFA56-4D83-4EFF-2B8F-08D6773CA7E6' THEN 'null Julian Rhodes' 
     WHEN [Id] = 'F68C8654-DFC2-4660-BB96-B82F25AFEC46' THEN 'null Jeremy Hay Campbell'
     WHEN [Id] = '686E8E82-810C-4D99-85FB-CCF2625E7792' THEN 'null Jessica Button'
     WHEN [Id] IN ( '19D57EAC-7626-4F63-996D-ED9E2602F6E7' , 'B8764FF2-4883-43A6-84FB-7FB6C85A77E6' , 'E85A99EC-323A-41B5-A328-E12036966407') THEN 'null ' + TRIM(DisplayName)
     ELSE TRIM(DisplayName) END DisplayName, Id, Email
 FROM [Contacts] co1 WHERE TRIM(DisplayName) IS NOT NULL
 ) t
    CROSS APPLY (SELECT CHARINDEX(' ', T.DisplayName, 1)) AS FirstSpace(i)
    CROSS APPLY (SELECT CHARINDEX(' ', T.DisplayName, FirstSpace.i + 1)) AS SecondSpace(j)
    CROSS APPLY (SELECT CHARINDEX(' ', T.DisplayName, SecondSpace.j + 1)) AS ThirdSpace(k)
    CROSS APPLY (SELECT CASE WHEN FirstSpace.i = 0 THEN LEN(T.DisplayName) ELSE FirstSpace.i - 1 END) LenOne(n)
    CROSS APPLY (SELECT CASE WHEN FirstSpace.i = 0 THEN 0
                             WHEN SecondSpace.j = 0 THEN LEN(T.DisplayName) - FirstSpace.i - 1 
                             ELSE SecondSpace.j - FirstSpace.i - 1 END ) LenTwo(l)
    CROSS APPLY (SELECT CASE WHEN FirstSpace.i = 0 THEN 0
                             WHEN SecondSpace.j = 0 THEN 0 
                             ELSE LEN(T.DisplayName) - SecondSpace.j END) LenThree(m)
    CROSS APPLY (SELECT CASE WHEN FirstSpace.i = 0 THEN T.DisplayName 
                             WHEN SecondSpace.j = 0 THEN SUBSTRING(T.Displayname, FirstSpace.i + 1, LEN(T.DisplayName) - FirstSpace.i) 
                             ELSE SUBSTRING(T.Displayname, SecondSpace.j + 1, LEN(T.DisplayName) - SecondSpace.j) END) FamilyName(f)
    CROSS APPLY (SELECT CASE WHEN FirstSpace.i = 0 THEN NULL
                             WHEN SecondSpace.j = 0 THEN SUBSTRING(T.Displayname, 1, FirstSpace.i) 
                             ELSE SUBSTRING(T.Displayname, FirstSpace.i + 1, SecondSpace.j - FirstSpace.i) END ) GivenNames(g)
) up1
ON (co1.Id = up1.Id)
WHEN MATCHED THEN UPDATE 
SET Title = up1.Title,
GivenNames = up1.GivenNames,
FamilyName = up1.FamilyName,
DisplayName = TRIM(up1.Title + (CASE WHEN up1.Title = '' THEN '' ELSE + ' ' END) + up1.GivenNames  + (CASE WHEN up1.GivenNames = '' THEN '' ELSE + ' ' END) + up1.FamilyName);

