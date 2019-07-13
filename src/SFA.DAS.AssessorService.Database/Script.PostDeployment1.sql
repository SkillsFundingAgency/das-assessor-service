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

IF NOT EXISTS (SELECT * FROM EMailTemplates WHERE TemplateName = N'EPAOPermissionsAmended')
BEGIN
INSERT EMailTemplates ([Id],[TemplateName],[TemplateId],[Recipients],[CreatedAt]) 
VALUES (NEWID(), N'EPAOPermissionsAmended', N'c1ba00d9-81b6-46d8-9b70-3d89d51aa9c1', NULL, GETDATE())
END

IF NOT EXISTS (SELECT * FROM EMailTemplates WHERE TemplateName = N'EPAOPermissionsRequested')
BEGIN
INSERT EMailTemplates ([Id],[TemplateName],[TemplateId],[Recipients],[CreatedAt]) 
VALUES (NEWID(), N'EPAOPermissionsRequested', N'addf58d9-9e20-46fe-b952-7fc62a47b7f7', NULL, GETDATE())
END

-- Update privileges
  
DECLARE @privilegesCount int
SELECT @privilegesCount = COUNT(*) FROM Privileges

IF (@privilegesCount = 6)
  BEGIN
    -- remove ContactsPrivileges records for API
    DELETE ContactsPrivileges
    FROM Privileges p
           INNER JOIN ContactsPrivileges cp ON cp.PrivilegeId = p.Id
    WHERE p.UserPrivilege = 'Manage API subscription'  
    
    DELETE Privileges WHERE UserPrivilege = 'Manage API subscription'
  END
  
IF (@privilegesCount < 5)
  BEGIN
    -- remove ContactsPrivileges records for View standards
    DELETE ContactsPrivileges
    FROM Privileges p
           INNER JOIN ContactsPrivileges cp ON cp.PrivilegeId = p.Id
    WHERE p.UserPrivilege = 'View standards'

    -- remove Privileges View standards
    DELETE Privileges WHERE UserPrivilege = 'View standards'

    -- rename existing privileges
    UPDATE Privileges SET UserPrivilege = 'Apply for a Standard', Description = 'This area allows you to apply for a standard.' WHERE UserPrivilege = 'Apply for standards'

    -- add new ones
    INSERT INTO Privileges (Id, UserPrivilege, Description) VALUES (NEWID(), 'View completed assessments', 'This area shows all previously recorded assessments.')
/*  Do not yet add API management
--  INSERT INTO Privileges (Id, UserPrivilege, Description) VALUES (NEWID(), 'Manage API subscription', 'This area allows you to manage your API subscriptions.')
*/
    INSERT INTO Privileges (Id, UserPrivilege, Description) VALUES (NEWID(), 'View pipeline', 'This area shows the Standard and number of apprentices due to be assessed.')

    -- set Manage Users to MustBeAtLeast.... true
    UPDATE Privileges SET MustBeAtLeastOneUserAssigned = 1, Description = 'This area shows a list of all users in your organisation and the ability to manage their permissions.' WHERE UserPrivilege = 'Manage users'
    
    UPDATE Privileges SET Description = 'This area allows you to record assessment grades and produce certificates.' WHERE UserPrivilege = 'Record grades and issue certificates'
  END  
  

DELETE EMailTemplates WHERE TemplateName = 'EPAOUserApproveRequest'
DELETE EMailTemplates WHERE TemplateName = 'EPAOUserApproveConfirm'
DELETE EMailTemplates WHERE TemplateName = 'EPAOUserApproveReject'

INSERT INTO EMailTemplates (Id, TemplateName, TemplateId, CreatedAt) VALUES (NEWID(), 'EPAOUserApproveRequest', 'f7ca95a9-54fb-4f5f-8a88-840445f98c8b', GETUTCDATE())
INSERT INTO EMailTemplates (Id, TemplateName, TemplateId, CreatedAt) VALUES (NEWID(), 'EPAOUserApproveConfirm', '68506adb-7e17-45c9-ad54-45ef9a2cad15', GETUTCDATE())
INSERT INTO EMailTemplates (Id, TemplateName, TemplateId, CreatedAt) VALUES (NEWID(), 'EPAOUserApproveReject', 'e7dc7016-9c88-4e25-9496-cb135001f413', GETUTCDATE())
  
  
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
update deliveryarea set Ordering=9 where Area='South West'*/

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

-- DONE
-- fix options
--UPDATE [Certificates]
--SET [CertificateData] = JSON_MODIFY([CertificateData], '$.CourseOption','Alcoholic Beverage Service') 
--WHERE json_value(certificatedata,'$.CourseOption') = 'Alcholic beverage service'

--UPDATE [Options] 
--SET [OptionName] = 'Alcoholic Beverage Service'
--WHERE [OptionName] = 'Alcholic beverage service'


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

/* DONE 
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
*/

/* DONE 
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

*/

/* DONE
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

*/

/* DONE
UPDATE [OrganisationType] SET [Type] =  'Training Provider', [TypeDescription] = 'Training provider - including HEI not in England' WHERE id = 7;
*/

-- ON-1933 INC INC01095552
DELETE FROM [Options] WHERE [StdCode] IN (50, 51);

	-- Options for: Chartered Surveyor (Degree) ST0331
INSERT INTO [Options] ([StdCode], [OptionName])
     VALUES
             (50, 'Building Surveyors')
		   , (50, 'Commercial Property Surveyors')
		   , (50, 'Residential Property Surveyors')
		   , (50, 'Planning and Development Surveyors')
		   , (50, 'Rural Surveyors')
		   , (50, 'Minerals and Waste Management Surveyors')
		   , (50, 'Valuation Surveyors')
		   , (50, 'Consultant (Professional) Quantity Surveyors')
		   , (50, 'Consultant (Professional) Project Management Surveyors');

	-- Options for: Surveying technician ST0332
INSERT INTO [Options] ([StdCode], [OptionName])
     VALUES
             (51, 'Building Surveying Technicians')
		   , (51, 'Commercial Property Surveying Technicians')
		   , (51, 'Residential Property Surveying Technicians')
		   , (51, 'Land Surveying Technicians (including rural, minerals and waste management and planning and development)')
		   , (51, 'Valuation Surveying Technicians')
		   , (51, 'Consultant (Professional) Quantity Surveying Technicians')
		   , (51, 'Consultant (Professional) Project Management Technicians');

UPDATE [Options] SET [OptionName] = 'Mechanical' WHERE [OptionName] = 'Mechnical';
-- END OF ON-1933


-- START OF ON-1981
UPDATE Certificates
SET CertificateData =	CASE JSON_Value(CertificateData,'$.EpaDetails.LatestEpaOutcome')
                            WHEN 'fail' THEN JSON_MODIFY(CertificateData,'$.EpaDetails.LatestEpaOutcome','fail')
							WHEN 'Fail' THEN JSON_MODIFY(CertificateData,'$.EpaDetails.LatestEpaOutcome','fail')
                            ELSE JSON_MODIFY(CertificateData,'$.EpaDetails.LatestEpaOutcome','pass')
						END
WHERE JSON_Value(CertificateData,'$.EpaDetails.LatestEpaOutcome') IS NOT NULL;

UPDATE [dbo].[Certificates]
SET [CertificateData] = REPLACE(CertificateData, '"EpaOutcome":"Fail"', '"EpaOutcome":"fail"')
WHERE JSON_Value(CertificateData,'$.EpaDetails.Epas[0].EpaOutcome') IS NOT NULL;

UPDATE [dbo].[Certificates]
SET [CertificateData] = REPLACE(REPLACE(REPLACE(CertificateData, '"EpaOutcome":"Pass"', '"EpaOutcome":"pass"'), '"EpaOutcome":"Credit"', '"EpaOutcome":"pass"'), '"EpaOutcome":"Merit"', '"EpaOutcome":"pass"')
WHERE JSON_Value(CertificateData,'$.EpaDetails.Epas[0].EpaOutcome') IS NOT NULL;

UPDATE [dbo].[Certificates]
SET [CertificateData] = REPLACE(REPLACE(REPLACE(CertificateData, '"EpaOutcome":"Distinction"', '"EpaOutcome":"pass"'), '"EpaOutcome":"Pass with excellence"', '"EpaOutcome":"pass"'), '"EpaOutcome":"No grade awarded"', '"EpaOutcome":"pass"')
WHERE JSON_Value(CertificateData,'$.EpaDetails.Epas[0].EpaOutcome') IS NOT NULL;
-- END OF ON-1981

-- START OF ON-1926
update [dbo].[Contacts] set [Username] = [Email] where  [Username] like 'unknown%' and [signinid] is not null
-- END OF ON-1926

-- START OF ON-2063
UPDATE Privileges SET Description = 'This area allows you to apply for a Standard.' WHERE UserPrivilege = 'Apply for a Standard'
-- END OF ON-2063

-- START OF ON-859
UPDATE Certificates SET IsPrivatelyFunded = 1 WHERE CertificateReferenceId in (00000009,00000010,00000011,00000012,00000013,00000014,00000015,00000016,00000017,00000018,00000019,00000020,00000021,00000022,00000023,
00000024,00000025,00000026,00000027,00000028,00000029,00000030,00000031,00000032,00000033,00000034,00000035,00000036,00000037,00000038,00000039,00000040,00000041,00000042,00000043,00000044,00000045,00000046,00000047,
00000048,00000049,00000050,00000051,00000052,00000053,00000054,00000055,00000056,00000057,00000058,00000059,00000060,00000061,00000062,00000063,00000064,00000065,00000066,00000067,00000068,00000069,00000070,00000071,
00000072,00000073,00000074,00000075,00000076,00000077,00000078,00000079,00000080,00000127,00000128,00000129,00000130,00000131,00000132,00000133,00000134,00000135,00000136,00000137,00000138,00000139,00000140,00000141,
00000142,00000143,00000144,00000145,00000146,00000147,00000148,00000149,00000150,00000151,00000152,00000153,00000154,00000155,00000156,00000157,00000158,00000159,00000160,00000224,00000225,00000226,00000227,00000228,
00000229,00000230,00000231,00000232,00000233,00000234,00000235,00000236,00000237,00000238,00000239,00000240,00000241,00000242,00000243,00000244,00000245,00000246,00000247,00000248,00000249,00000250,00000251,00000252,
00000253,00000254,00000255,00000256,00000257,00000258,00000259,00000260,00000261,00000262,00000263,00000264,00000265,00000266,00000267,00000268,00000349,00000350,00000351,00000352,00000353,00000354,00000355,00000356,
00000357,00000358,00000359,00000360,00000361,00000362,00000363,00000364,00000365,00000366,00000367,00000368,00000369,00000370,00000371,00000372,00000373,00000374,00000375,00000376,00000377,00000378,00000379,00000380,
00000381,00000382,00000383,00000384,00000385,00000386,00000387,00000388,00000389,00000390,00000391,00000392,00000393,00000394,00000395,00000396,00000397,00000398,00000399,00000400,00000401,00000402,00000403,00000404,
00000405,00000406,00000407,00000408,00000409,00000410,00000411,00000412,00000413,00000414,00000415,00000416,00000417,00000418,00000419,00000420,00000421,00000422,00000423,00000424,00000425,00000426,00000427,00000428,
00000429,00000430,00000431,00000432,00000433,00000434,00000435,00000436,00000437,00000438,00000439,00000440,00000441,00000442,00000443,00000444,00000445,00000446,00000447,00000448,00000449,00000450,00000507,00000508,
00000509,00000510,00000511,00000512,00000513,00000514,00000515,00000516,00000517,00000518,00000519,00000520,00000521,00000522,00000523,00000524,00000525,00000526,00000527,00000528,00000529,00000530,00000531,00000532,
00000533,00000534,00000535,00000536,00000537,00000538,00000539,00000540,00000541,00000579,00000580,00000581,00000582,00000583,00000584,00000585,00000586,00000587,00000588,00000589,00000590,00000591,00000592,00000593,
00000594,00000595,00000596,00000597,00000598,00000599,00000600,00000601,00000602,00000603,00000604,00000605,00000606,00000707,00000708,00000709,00000710,00000711,00000712,00000713,00000714,00000715,00000716,00000717,
00000718,00000719,00000720,00000721,00000722,00000723,00000724,00000725,00000726,00000727,00000728,00000729,00000730,00000731,00000732,00000733,00000734,00000735,00000736,00000737,00000738,00000739,00000740,00000741,
00000742,00000743,00000744,00000745,00000746,00000747,00000748,00000749,00000750,00000751,00000752,00001009,00001010,00001011,00001012,00001013,00001014,00001015,00001016,00001017,00001018,00001019,00001020,00001021,
00001022,00001023,00001355,00001356,00001357,00001358,00001359,00001360,00001361,00001362,00001363,00001364,00001365,00001366,00001367,00001368,00001369,00001370,00001371,00001372,00001373,00001374,00001375,00001376,
00001377,00001378,00001379,00001380,00001381,00001382,00001383,00001384,00001385,00001386,00001387,00001388,00001389,00001390,00001391,00001392,00001393,00001394,00001395,00001396,00001397,00001398,00001399,00001400,
00001401,00001402,00001403,00001404,00001405,00001406,00001407,00001408,00001409,00001410,00001411,00001412,00001413,00001414,00001415,00001416,00001417,00001418,00001419,00001420,00001421,00001422,00001423,00001424,
00001425,00001426,00001866,00001867,00001868,00001869,00001870,00001871,00001872,00001873,00001874,00001875,00001876,00001877,00001878,00001879,00001880,00001881,00001882,00001883,00001884,00001885,00001886,00001887,
00001888,00001889,00001890,00001891,00001892,00001893,00001894,00001895,00001896,00001897,00001898,00001899,00001900,00001901,00001902,00001903,00001904,00001905,00001906,00001907,00001908,00001909,00001910,00001911,
00001912,00001913,00001914,00001915,00001916,00001917,00001918,00001919,00001976,00001977,00001978,00001979,00001980,00001981,00001982,00001983,00001984,00001985,00001986,00001987,00001988,00001989,00001990,00001991,
00001992,00001993,00001994,00001995,00001996,00001997,00001998,00001999,00002000,00002001,00002002,00002003,00002004,00002005,00002006,00002007,00002008,00002009,00002010,00002011,00002012,00002013,00002014,00002015,
00002016,00002017,00002018,00002019,00002020,00002021,00002022,00002023,00002024,00002025,00002026,00002027,00002028,00002029,00002030,00002031,00002032,00002033,00002034,00002035,00002036,00002037,00002038,00002039,
00002040,00002041,00002042,00002043,00002044,00002045,00002046,00002047,00002048,00002049,00002050,00002051,00002052,00002053,00002054,00002055,00002056,00002057,00002058,00002059,00002060,00002061,00002062,00002063,
00002064,00002065,00002066,00002067,00002068,00002069,00002070,00002071,00002072,00002073,00002074,00002075,00002076,00002077,00002078,00002079,00002080,00002081,00002082,00002083,00002084,00002085,00002086,00002087,
00002088,00002089,00002090,00002091,00002092,00002093,00002094,00002095,00002096,00002097,00002098,00002099,00002100)
-- END OF ON-859