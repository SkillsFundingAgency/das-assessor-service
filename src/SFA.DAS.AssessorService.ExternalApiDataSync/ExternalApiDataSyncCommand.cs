using Dapper;
using SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure;
using SFA.DAS.AssessorService.ExternalApiDataSync.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace SFA.DAS.AssessorService.ExternalApiDataSync
{
    public interface ICommand
    {
        Task Execute();
    }

    public class ExternalApiDataSyncCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly bool _allowDataSync;
        private readonly string _sourceConnectionString;
        private readonly string _destinationConnectionString;

        private readonly SqlBulkCopyOptions _bulkCopyOptions;

        public ExternalApiDataSyncCommand(IWebConfiguration config, IAggregateLogger aggregateLogger)
        {
            _aggregateLogger = aggregateLogger;

            _allowDataSync = config.ExternalApiDataSync.IsEnabled;
            _sourceConnectionString = config.SqlConnectionString;
            _destinationConnectionString = config.SandboxSqlConnectionString;
            _bulkCopyOptions = SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock;
        }

        public async Task Execute()
        {
            _aggregateLogger.LogInformation("External Api Data Sync Function Started");
            _aggregateLogger.LogInformation($"Process Environment = {EnvironmentVariableTarget.Process}");

            if (_allowDataSync)
            {
                try
                {
                    _aggregateLogger.LogInformation("Proceeding with External Api Data Sync...");
                    Step0_TearDown_Database();
                    Step1_Organisation_Data();
                    Step2_Contacts_Data();
                    Step3_Standard_Data();
                    Step4_OrganisationStandard_Data();
                    Step5_Obfuscate_Personal_Data();
                    Step6_Generate_Test_Data();
                    _aggregateLogger.LogInformation("External Api Data Sync completed");
                }
                catch (TransactionAbortedException ex)
                {
                    _aggregateLogger.LogError(ex, "Transaction was aborted occurred during External Api Data Sync");
                }
                catch (SqlException ex)
                {
                    _aggregateLogger.LogError(ex, "SqlException occurred during External Api Data Sync");
                }
                catch (Exception ex)
                {
                    _aggregateLogger.LogError(ex, "Unknown Error occurred during External Api Data Sync");
                }
            }
            else
            {
                _aggregateLogger.LogInformation("External Api Data Sync is disabled at this time");
            }

            await Task.CompletedTask;
        }

        private void Step0_TearDown_Database()
        {
            _aggregateLogger.LogInformation("Step 0: Tear Down Database");

            using (var trans = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(_destinationConnectionString))
                {
                    if (!conn.State.HasFlag(ConnectionState.Open)) conn.Open();

                    // repopulated in Step 6
                    conn.Execute(
                        @"  DELETE FROM CertificateLogs;
                            DELETE FROM Certificates;
                            DELETE FROM Ilrs;
                            DBCC CHECKIDENT('Certificates', RESEED, 10000);");

                    // repopulated in Step 4
                    conn.Execute(
                        @"  DELETE FROM OrganisationStandardDeliveryArea;
                            DELETE FROM OrganisationStandard;
                            DELETE FROM DeliveryArea;
                            DBCC CHECKIDENT('OrganisationStandardDeliveryArea', RESEED, 0);
                            DBCC CHECKIDENT('OrganisationStandard', RESEED, 0);
                            DBCC CHECKIDENT('DeliveryArea', RESEED, 0);");

                    // repopulated in Step 3
                    conn.Execute(
                        @"  DELETE FROM Options;
                            DELETE FROM StandardCollation;");

                    // repopulated in Step 2
                    conn.Execute(
                        @"  DELETE FROM ContactLogs;
                            DELETE FROM ContactsPrivileges;
                            DELETE FROM ContactRoles;
                            DELETE FROM Contacts;");

                    // repopulated in Step 1
                    conn.Execute(
                        @"  DELETE FROM Organisations;
                            DELETE FROM OrganisationType;
                            DBCC CHECKIDENT('OrganisationType', RESEED, 0);");
                }

                trans.Complete();
            }

            _aggregateLogger.LogInformation("Step 0: Tear Down Completed");
        }

        private void Step1_Organisation_Data()
        {
            _aggregateLogger.LogInformation("Step 1: Syncing Organisation Data");
            BulkCopyData(new List<string> { "OrganisationType", "Organisations" });
            _aggregateLogger.LogInformation("Step 1: Completed");
        }

        private void Step2_Contacts_Data()
        {
            _aggregateLogger.LogInformation("Step 2: Syncing Contacts");
            BulkCopyData(new List<string> { "Contacts" });
            _aggregateLogger.LogInformation("Step 2: Completed");
        }

        private void Step3_Standard_Data()
        {
            _aggregateLogger.LogInformation("Step 3: Syncing Standard Data");
            BulkCopyData(new List<string> { "StandardCollation", "Options" });
            _aggregateLogger.LogInformation("Step 3: Completed");
        }

        private void Step4_OrganisationStandard_Data()
        {
            _aggregateLogger.LogInformation("Step 4: Syncing Organisation Standard Data");
            BulkCopyData(new List<string> { "DeliveryArea", "OrganisationStandard", "OrganisationStandardDeliveryArea" });
            _aggregateLogger.LogInformation("Step 4: Completed");
        }

        private void Step5_Obfuscate_Personal_Data()
        {
            _aggregateLogger.LogInformation("Step 5: Obfuscate Personal Data");

            using (var trans = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(_destinationConnectionString))
                {
                    if (!conn.State.HasFlag(ConnectionState.Open)) conn.Open();

                    conn.Execute(@" UPDATE Contacts
                                    SET GivenNames = ISNULL(EndPointAssessorOrganisationId, 'UNKNOWN')
                                        , FamilyName = 'TEST'
                                        , DisplayName = ISNULL(EndPointAssessorOrganisationId, 'UNKNOWN') + ' TEST'
                                        , Title = ''
                                        , Email = ISNULL(EndPointAssessorOrganisationId, 'UNKNOWN') + '@TEST.TEST'
                                        , PhoneNumber = NULL
                                        , SignInId = NULL;");
                }

                trans.Complete();
            }

            _aggregateLogger.LogInformation("Step 5: Completed");
        }

        private void Step6_Generate_Test_Data()
        {
            _aggregateLogger.LogInformation("Step 6: Generating Test Data");

            using (var trans = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(_destinationConnectionString))
                {
                    if (!conn.State.HasFlag(ConnectionState.Open)) conn.Open();

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

            _aggregateLogger.LogInformation("Step 6: Completed");
        }

        private void BulkCopyData(List<string> tablesToCopy)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlbulkcopy
            if (tablesToCopy is null) throw new ArgumentNullException(nameof(tablesToCopy));

            using (SqlConnection source = new SqlConnection(_sourceConnectionString), dest = new SqlConnection(_destinationConnectionString))
            {
                if (!source.State.HasFlag(ConnectionState.Open)) source.Open();
                if (!dest.State.HasFlag(ConnectionState.Open)) dest.Open();

                using (var transaction = dest.BeginTransaction())
                {
                    foreach (var table in tablesToCopy)
                    {
                        _aggregateLogger.LogDebug($"\tSyncing table: {table}");
                        using (var commandSourceData = new SqlCommand($"SELECT * FROM {table} ORDER BY [Id]", source))
                        {
                            using (var reader = commandSourceData.ExecuteReader())
                            {
                                using (var bulkCopy = new SqlBulkCopy(dest, _bulkCopyOptions, transaction))
                                {
                                    bulkCopy.DestinationTableName = table;
                                    bulkCopy.WriteToServer(reader);
                                }
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
        }
    }
}

