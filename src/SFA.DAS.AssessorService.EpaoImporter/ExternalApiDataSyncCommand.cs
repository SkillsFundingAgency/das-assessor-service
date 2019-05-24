using Dapper;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;
using SFA.DAS.AssessorService.EpaoImporter.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.Settings;
using SqlBulkTools;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class ExternalApiDataSyncCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly bool _allowDataSync;
        private readonly string _sourceConnectionString;
        private readonly string _destinationConnectionString;

        public ExternalApiDataSyncCommand(IWebConfiguration config, IAggregateLogger aggregateLogger)
        {
            _aggregateLogger = aggregateLogger;

            _allowDataSync = false; // TODO: PLACE THIS INTO config
            _sourceConnectionString = ""; // TODO: PLACE THIS INTO config
            _destinationConnectionString = config.SqlConnectionString;

            SqlMapper.AddTypeHandler(typeof(StandardData), new StandardDataHandler());
            SqlMapper.AddTypeHandler(typeof(OrganisationStandardData), new OrganisationStandardDataHandler());
        }

        public async Task Execute()
        {
            _aggregateLogger.LogInfo("External Api Data Sync Function Started");
            _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

            if (_allowDataSync)
            {
                _aggregateLogger.LogInfo("Proceeding with External Api Data Sync...");
                await Step1_Organisation_Data();
                await Step2_Contacts_Data();
                await Step3_Standard_Data();
                await Step4_OrganisationStandard_Data();
                Step5_Generate_Test_Data();
            }
            else
            {
                _aggregateLogger.LogInfo("External Api Data Sync is disabled at this time");
            }
        }


        public async Task Step1_Organisation_Data()
        {
            _aggregateLogger.LogInfo("Step 1: Syncing Organisation Data");

            var orgTypes = new List<OrganisationType>();
            var orgs = new List<Organisation>();

            using (var sourceConnection = new SqlConnection(_sourceConnectionString))
            {
                orgTypes = (await sourceConnection.QueryAsync<OrganisationType>("SELECT * FROM OrganisationType ORDER BY [Id]")).ToList();
                orgs = (await sourceConnection.QueryAsync<Organisation>("SELECT * FROM Organisations ORDER BY [Id]")).ToList();
            }

            var bulk = new BulkOperations();
            using (var trans = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(_destinationConnectionString))
                {
                    conn.Open();

                    bulk.Setup<OrganisationType>()
                        .ForCollection(orgTypes)
                        .WithTable("OrganisationType")
                        .WithBulkCopySettings( new BulkCopySettings { SqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity })
                        .AddAllColumns()
                        .BulkInsertOrUpdate()
                        .SetIdentityColumn(x => x.Id)
                        .MatchTargetOn(x => x.Id)
                        .DeleteWhenNotMatched(true)
                        .Commit(conn);

                    bulk.Setup<Organisation>()
                        .ForCollection(orgs)
                        .WithTable("Organisations")
                        .AddAllColumns()
                        .BulkInsertOrUpdate()
                        .MatchTargetOn(x => x.Id)
                        .Commit(conn);
                }

                trans.Complete();
            }
        }

        public async Task Step2_Contacts_Data()
        {
            _aggregateLogger.LogInfo("Step 2: Syncing Contacts");

            var contacts = new List<Contact>();

            using (var sourceConnection = new SqlConnection(_sourceConnectionString))
            {
                contacts = (await sourceConnection.QueryAsync<Contact>("SELECT * FROM Contacts ORDER BY [Id]")).ToList();
            }

            var bulk = new BulkOperations();
            using (var trans = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(_destinationConnectionString))
                {
                    bulk.Setup<Contact>()
                        .ForCollection(contacts)
                        .WithTable("Contacts")
                        .AddAllColumns()
                        .BulkInsertOrUpdate()
                        .MatchTargetOn(x => x.Id)
                        .Commit(conn);

                    // Obfuscate_Personal_Data
                    conn.Execute(@" UPDATE Contacts
                                    SET GivenNames = 'TEST'
                                        , FamilyName = 'TEST'
                                        , DisplayName = 'TEST TEST'
                                        , Email = 'TEST@TEST.TEST'
                                        , PhoneNumber = NULL
                                        , SignInId = NULL;");
                }

                trans.Complete();
            }
        }

        public async Task Step3_Standard_Data()
        {
            _aggregateLogger.LogInfo("Step 3: Syncing Standard Data");

            var standards = new List<StandardCollation>();
            var options = new List<Option>();

            using (var sourceConnection = new SqlConnection(_sourceConnectionString))
            {
                standards = (await sourceConnection.QueryAsync<StandardCollation>("SELECT * FROM StandardCollation ORDER BY [Id]")).ToList();
                options = (await sourceConnection.QueryAsync<Option>("SELECT * FROM Options ORDER BY [Id]")).ToList();
            }

            var bulk = new BulkOperations();
            using (var trans = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(_destinationConnectionString))
                {
                    bulk.Setup<StandardCollation>()
                        .ForCollection(standards)
                        .WithTable("StandardCollation")
                        .WithBulkCopySettings(new BulkCopySettings { SqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity })
                        .AddAllColumns()
                        .BulkInsertOrUpdate()
                        .SetIdentityColumn(x => x.Id)
                        .MatchTargetOn(x => x.Id)
                        .DeleteWhenNotMatched(true)
                        .Commit(conn);

                    bulk.Setup<Option>()
                        .ForCollection(options)
                        .WithTable("Options")
                        .AddAllColumns()
                        .BulkInsertOrUpdate()
                        .MatchTargetOn(x => x.Id)
                        .DeleteWhenNotMatched(true)
                        .Commit(conn);
                }

                trans.Complete();
            }
        }

        public async Task Step4_OrganisationStandard_Data()
        {
            _aggregateLogger.LogInfo("Step 4: Syncing Organisation Standard Data");

            var deliveryArea = new List<DeliveryArea>();
            var orgStandard = new List<EpaOrganisationStandard>();
            var orgStandardDeliveryArea = new List<EpaOrganisationStandardDeliveryArea>();

            using (var sourceConnection = new SqlConnection(_sourceConnectionString))
            {
                deliveryArea = (await sourceConnection.QueryAsync<DeliveryArea>("SELECT * FROM DeliveryArea ORDER BY [Id]")).ToList();
                orgStandard = (await sourceConnection.QueryAsync<EpaOrganisationStandard>("SELECT * FROM OrganisationStandard ORDER BY [Id]")).ToList();
                orgStandardDeliveryArea = (await sourceConnection.QueryAsync<EpaOrganisationStandardDeliveryArea>("SELECT * FROM OrganisationStandardDeliveryArea ORDER BY [Id]")).ToList();
            }

            var bulk = new BulkOperations();
            using (var trans = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(_destinationConnectionString))
                {
                    bulk.Setup<DeliveryArea>()
                        .ForCollection(deliveryArea)
                        .WithTable("DeliveryArea")
                        .WithBulkCopySettings(new BulkCopySettings { SqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity })
                        .AddAllColumns()
                        .BulkInsertOrUpdate()
                        .SetIdentityColumn(x => x.Id)
                        .MatchTargetOn(x => x.Id)
                        .DeleteWhenNotMatched(true)
                        .Commit(conn);

                    // NOTE: Have to delete these as IDENTITY insert isn't working correctly (maybe because the Id's don't start at 1 due to spreadsheet import)
                    conn.Execute("DELETE FROM OrganisationStandardDeliveryArea;");

                    bulk.Setup<EpaOrganisationStandard>()
                        .ForCollection(orgStandard)
                        .WithTable("OrganisationStandard")
                        .WithBulkCopySettings(new BulkCopySettings { SqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity })
                        .AddAllColumns()
                        .RemoveColumn(x => x.ContactEmail)
                        .RemoveColumn(x => x.ContactName)
                        .RemoveColumn(x => x.ContactPhoneNumber)
                        .BulkInsertOrUpdate()
                        .SetIdentityColumn(x => x.Id)
                        .MatchTargetOn(x => x.Id)
                        .DeleteWhenNotMatched(true)
                        .Commit(conn);

                    // NOTE: As the Id's won't match, we cannot do this...
                    //    bulk.Setup<EpaOrganisationStandardDeliveryArea>()
                    //        .ForCollection(orgStandardDeliveryArea)
                    //        .WithTable("OrganisationStandardDeliveryArea")
                    //        .WithBulkCopySettings(new BulkCopySettings { SqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity })
                    //        .AddAllColumns()
                    //        .RemoveColumn(x => x.EndPointAssessorOrganisationId)
                    //        .RemoveColumn(x => x.StandardCode)
                    //        .BulkInsertOrUpdate()
                    //        .SetIdentityColumn(x => x.Id)
                    //        .MatchTargetOn(x => x.Id)
                    //        .DeleteWhenNotMatched(true)
                    //        .Commit(conn);
                    // ... so we have to generate fake data instead
                    conn.Execute(@" INSERT INTO OrganisationStandardDeliveryArea
                                           (OrganisationStandardId, DeliveryAreaId, Comments, Status)
	                                SELECT os.Id, da.Id AS DeliveryAreaId, NULL AS Comments, os.Status
	                                FROM OrganisationStandard os, DeliveryArea da
	                                WHERE os.Status = 'Live' AND da.Status = 'Live';");
                }

                trans.Complete();
            }
        }

        public void Step5_Generate_Test_Data()
        {
            _aggregateLogger.LogInfo("Step 5: Generating Test Data");

            using (var trans = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(_destinationConnectionString))
                {
                    conn.Open();

                    conn.Execute(@"DELETE FROM Ilrs
                                    WHERE GivenNames = 'Test' AND FamilyName LIKE '1%';");

                    conn.Execute(@"DELETE FROM CertificateLogs
                                    WHERE CertificateId IN (
                                        SELECT Id FROM[Certificates]
                                            WHERE JSON_VALUE(CertificateData, '$.LearnerFamilyName') LIKE '1%' AND
                                                  JSON_VALUE(CertificateData, '$.LearnerGivenNames') = 'Test'
                                        );");

                    conn.Execute(@"DELETE FROM Certificates
                                    WHERE JSON_VALUE(CertificateData, '$.LearnerFamilyName') LIKE '1%' AND
                                          JSON_VALUE(CertificateData, '$.LearnerGivenNames') = 'Test';");

                    conn.Execute(
                        @"WITH CTE AS (
                          SELECT 0 as Number
                          UNION ALL
                          SELECT Number+1
                          FROM CTE 
                          WHERE Number < 9 
                        )
                        INSERT INTO [Ilrs](Id, CreatedAt, Uln, FamilyName ,GivenNames, UkPrn, StdCode, LearnStartDate, EpaOrgId, FundingModel, ApprenticeshipId, EmployerAccountId, Source, LearnRefNumber, CompletionStatus, EventId, PlannedEndDate)
                        SELECT
                          NEWID() AS Id,
                          GETDATE() AS CreatedAt,
                          CONVERT(BIGINT, ab1.Uln) AS Uln,
                          ab1.Uln AS FamilyName,
                          'Test' AS GivenNames,
                          ab1.UkPrn AS UkPrn,
                          ab1.StandardCode AS StdCode,
                          EOMONTH(DATEADD(MONTH, 0 - ab1.Duration, GETDATE())) AS LearnStartDate, 
                          ab1.EndPointAssessorOrganisationId AS EPAOrgId,
                          36 AS FundingModel,
                          NULL AS ApprenticeshipId,
                          0 AS EmployerAccountId,
                          CONVERT(CHAR(2),DATEADD(MONTH, -12, GETDATE()),2) + CONVERT(CHAR(2),GETDATE(),2) AS Source,
                          'A' + ab1.Uln AS LearnRefNumber,
                          1 AS CompletionStatus,
                          NULL AS EventId,
                          GETDATE() AS PlannedEndDate
                        FROM (
                          SELECT 
                            '1'+ SUBSTRING(ogs.EndPointAssessorOrganisationId,4,4) + RIGHT('000'+CAST(ogs.StandardCode AS VARCHAR(3)),3) +RIGHT('00'+CAST(CTE.Number AS VARCHAR(2)),2) AS Uln, 
	                        og1.EndPointAssessorUkprn AS UkPrn,
	                        ogs.EndPointAssessorOrganisationId AS EndPointAssessorOrganisationId,
                            ogs.StandardCode,
	                        CTE.*,
	                        CONVERT(NUMERIC, JSON_VALUE(sc1.StandardData,'$.Duration')) AS Duration 
                        FROM CTE
                          CROSS JOIN OrganisationStandard ogs 
                          JOIN Organisations og1 ON og1.EndPointAssessorOrganisationId = ogs.EndPointAssessorOrganisationId AND og1.Status <> 'Deleted'
                          JOIN StandardCollation sc1 ON ogs.StandardCode = sc1.StandardId
                        WHERE  ogs.Status NOT IN ( 'Deleted','New') AND (ogs.EffectiveTo IS NULL OR ogs.EffectiveTo > GETDATE()) AND og1.EndPointAssessorUkprn IS NOT NULL
                        ) ab1
                        ORDER BY Uln, EndPointAssessorOrganisationId, StandardCode, Number");
                }

                trans.Complete();
            }
        }
    }
}
