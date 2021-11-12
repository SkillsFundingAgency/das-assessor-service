using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Data
{
    public class SandboxDbRepository : Repository, ISandboxDbRepository
    {
        private readonly SqlBulkCopyOptions _bulkCopyOptions;
        private readonly IWebConfiguration _config;
        private readonly ILogger<SandboxDbRepository> _logger;

        private const string AzureResource = "https://database.windows.net/";

        public SandboxDbRepository(IUnitOfWork unitOfWork, IWebConfiguration config, ILogger<SandboxDbRepository> logger)
            : base(unitOfWork)
        {
            _config = config;
            _logger = logger;
            _bulkCopyOptions = SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls |
                               SqlBulkCopyOptions.TableLock;
        }

        public async Task RebuildExternalApiSandbox()
        {
            try
            {
                _logger.LogInformation("Proceeding with rebuilding of ExternalApiSandbox...");

                using (var destinationSqlConnection = SandboxSqlConnection(_config.SandboxSqlConnectionString))
                {
                    if (!destinationSqlConnection.State.HasFlag(ConnectionState.Open)) destinationSqlConnection.Open();

                    using (var transaction = destinationSqlConnection.BeginTransaction())
                    {
                        Step0_TearDown_Database(transaction);
                        Step1_Organisation_Data(transaction);
                        Step2_Contacts_Data(transaction);
                        Step3_Standard_Data(transaction);
                        Step4_OrganisationStandard_Data(transaction);
                        Step5_Obfuscate_Personal_Data(transaction);
                        Step6_Generate_Test_Data(transaction);

                        transaction.Commit();
                    }
                }

                _logger.LogInformation("Rebuilding of ExternalApiSandbox completed");
            }
            catch (TransactionAbortedException ex)
            {
                _logger.LogError(ex, "Transaction was aborted during Rebuilding of ExternalApiSandbox");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SqlException occurred during Rebuilding of ExternalApiSandbox");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown Error occurred during Rebuilding of ExternalApiSandbox");
            }
            await Task.CompletedTask;
        }

        private void Step0_TearDown_Database(SqlTransaction transaction)
        {
            _logger.LogInformation("Step 0: Tear Down Database");

            // repopulated in Step 6
            transaction.Connection.Execute(
                @"  DELETE FROM CertificateLogs;
                            DELETE FROM Certificates;
                            DELETE FROM Ilrs;
                            DBCC CHECKIDENT('Certificates', RESEED, 10001);", transaction: transaction);

            // repopulated in Step 4
            transaction.Connection.Execute(
                @"  DELETE FROM OrganisationStandardDeliveryArea;
                            DELETE FROM OrganisationStandard;
                            DELETE FROM OrganisationStandardVersion;
                            DELETE FROM DeliveryArea;
                            DBCC CHECKIDENT('OrganisationStandardDeliveryArea', RESEED, 1);
                            DBCC CHECKIDENT('OrganisationStandard', RESEED, 1);
                            DBCC CHECKIDENT('DeliveryArea', RESEED, 1);", transaction: transaction);

            // repopulated in Step 3
            transaction.Connection.Execute(
                @"  DELETE FROM Standards;
                            DELETE FROM StandardOptions;", transaction: transaction);

            // repopulated in Step 2
            transaction.Connection.Execute(
                @"  DELETE FROM ContactLogs;
                            DELETE FROM ContactsPrivileges;
                            DELETE FROM Contacts;", transaction: transaction);

            // repopulated in Step 1
            transaction.Connection.Execute(
                @"  DELETE FROM Organisations;
                            DELETE FROM OrganisationType;
                            DBCC CHECKIDENT('OrganisationType', RESEED, 1);", transaction: transaction);

            _logger.LogInformation("Step 0: Tear Down Completed");
        }

        private void Step1_Organisation_Data(SqlTransaction transaction)
        {
            _logger.LogInformation("Step 1: Syncing Organisation Data");
            BulkCopyData(transaction, new List<string> { "OrganisationType", "Organisations" });
            _logger.LogInformation("Step 1: Completed");
        }

        private void Step2_Contacts_Data(SqlTransaction transaction)
        {
            _logger.LogInformation("Step 2: Syncing Contacts");
            BulkCopyData(transaction, new List<string> { "Contacts" });
            _logger.LogInformation("Step 2: Completed");
        }

        private void Step3_Standard_Data(SqlTransaction transaction)
        {
            _logger.LogInformation("Step 3: Syncing Standard Data");
            BulkCopyData(transaction, new List<string> { "Standards", "StandardOptions" });
            _logger.LogInformation("Step 3: Completed");
        }

        private void Step4_OrganisationStandard_Data(SqlTransaction transaction)
        {
            _logger.LogInformation("Step 4: Syncing Organisation Standard Data");
            BulkCopyData(transaction, new List<string> { "DeliveryArea", "OrganisationStandard", "OrganisationStandardVersion", "OrganisationStandardDeliveryArea" });
            _logger.LogInformation("Step 4: Completed");
        }

        private void Step5_Obfuscate_Personal_Data(SqlTransaction transaction)
        {
            _logger.LogInformation("Step 5: Obfuscate Personal Data");

            transaction.Connection.Execute(@" UPDATE Contacts
                                    SET GivenNames = ISNULL(EndPointAssessorOrganisationId, 'UNKNOWN')
                                        , FamilyName = 'TEST'
                                        , DisplayName = ISNULL(EndPointAssessorOrganisationId, 'UNKNOWN') + ' TEST'
                                        , Title = ''
                                        , Email = CONVERT(VARCHAR(36), Id) + '@TEST.TEST'
                                        , Username = CONVERT(VARCHAR(36), Id) + '@TEST.TEST'
                                        , PhoneNumber = NULL
                                        , SignInId = NULL;", transaction: transaction);

            transaction.Connection.Execute(@" UPDATE Organisations
                                    SET PrimaryContact = NULL
                                        , ApiUser = NULL;", transaction: transaction);

            _logger.LogInformation("Step 5: Completed");
        }

        private void Step6_Generate_Test_Data(SqlTransaction transaction)
        {
            _logger.LogInformation("Step 6: Generating Test Data");

            transaction.Connection.Execute(
                @"WITH CTE AS (
                          SELECT 0 as Number
                          UNION ALL
                          SELECT Number+1
                          FROM CTE 
                          WHERE Number < 9 
                        ),
                WITH Standards_CTE as(
                SELECT ROW_NUMBER() OVER (PARTITION BY Ifatereferencenumber ORDER BY VersionMajor, VersionMinor) seq, * from Standards)

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
                          JOIN Standards_CTE scte ON ogs.StandardCode = scte.LarsCode
                        WHERE  ogs.Status NOT IN ( 'Deleted','New') AND (ogs.EffectiveTo IS NULL OR ogs.EffectiveTo > GETDATE()) AND og1.EndPointAssessorUkprn IS NOT NULL
                        ) ab1
                        ORDER BY Uln, EndPointAssessorOrganisationId, StandardCode, Number", transaction: transaction);

            _logger.LogInformation("Step 6: Completed");
        }

        private void BulkCopyData(SqlTransaction transaction, List<string> tablesToCopy)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlbulkcopy
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));
            if (tablesToCopy is null) throw new ArgumentNullException(nameof(tablesToCopy));



            foreach (var table in tablesToCopy)
            {
                _logger.LogDebug($"\tSyncing table: {table}");

                using (var commandSourceData = new SqlCommand(
                    @"SELECT @subQuery = 'SELECT @sort_column = MAX(column_name) FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE table_name = ''' + @tableName + ''' AND column_name IN ( ''StandardUId'' , ''Id'')';

                        EXEC sp_executesql @subQuery, N'@sort_column varchar(100) out', @sort_column out

                        SELECT @sqlQuery = 'SELECT * FROM ' + @tableName + ' ORDER BY ' + @sort_column;
                        Exec(@sqlQuery)", _unitOfWork.Connection as SqlConnection))
                {
                    commandSourceData.Parameters.AddWithValue("@tableName", table);
                    commandSourceData.Parameters.AddWithValue("@subQuery", string.Empty);
                    commandSourceData.Parameters.AddWithValue("@sqlQuery", string.Empty);
                    commandSourceData.Parameters.AddWithValue("@sort_column", string.Empty);

                    if (!_unitOfWork.Connection.State.HasFlag(ConnectionState.Open)) _unitOfWork.Connection.Open();

                    using (var reader = commandSourceData.ExecuteReader())
                    {
                        using (var bulkCopy = new SqlBulkCopy(transaction.Connection, _bulkCopyOptions, transaction))
                        {
                            bulkCopy.DestinationTableName = table;
                            bulkCopy.WriteToServer(reader);
                        }
                    }
                }
            }
        }

        private SqlConnection SandboxSqlConnection(string sqlConnectionString)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return new SqlConnection
            {
                ConnectionString = sqlConnectionString,
                AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
            };
        }
    }
}

