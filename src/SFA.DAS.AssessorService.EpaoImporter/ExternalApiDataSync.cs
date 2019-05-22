using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Startup;
using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class ExternalApiDataSync
    {
        [FunctionName("ExternalApiDataSync")]
        public static void Run([TimerTrigger("0 0 0 1 1/1 *",RunOnStartup = true)] TimerInfo myTimer, TraceWriter functionLogger, ExecutionContext context)
        {

            // NOTE: THIS IS WORK IN PROGRESS. SqlBulkCopy only allows INSERTS which means totally trashing the existing database and reseeding INDENTY columns.
            // Get the destination database string wrong and it's game over!!!
            // SO... I'm going to have to do something which allows selecting into a temp table and then merging

            //var bootstrapper = new ExternalApiDataSyncBootstrapper(functionLogger, context);

            //var command = bootstrapper.GetInstance<ExternalApiDataSyncCommand>();
            //command.Execute().GetAwaiter().GetResult();


        }
    }

    public class ExternalApiDataSyncCommand : ICommand
    {
        private readonly IWebConfiguration _config;
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IAssessorServiceApi _assessorServiceApi;

        public ExternalApiDataSyncCommand(IWebConfiguration config, IAggregateLogger aggregateLogger, IAssessorServiceApi assessorServiceApi)
        {
            _config = config;
            _aggregateLogger = aggregateLogger;
            _assessorServiceApi = assessorServiceApi;
        }

        public Task Execute()
        {
            _aggregateLogger.LogInfo("External Api Data Sync Function Started");
            _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

            TearDown_Database();
            Step1_Organisation_Data();
            Step2_Contacts_Data();
            Step3_Standard_Data();
            Step4_OrganisationStandard_Data();
            Step5_Obfuscate_Personal_Data();
            Step6_Generate_Test_Data();

            return Task.CompletedTask;
        }

        public void TearDown_Database()
        {
            _aggregateLogger.LogInfo("Tear Down Database");

            using (var sourceConnection = new SqlConnection(_config.SqlConnectionString))
            {
                sourceConnection.Open();

                var teardownCommand = new SqlCommand(
                    @"  DELETE FROM CertificateLogs;
                        DELETE FROM Certificates;
                        DBCC CHECKIDENT('Certificates', RESEED, 10000);
                        DELETE FROM Ilrs;
                        DELETE FROM OrganisationStandardDeliveryArea;
                        DBCC CHECKIDENT('OrganisationStandardDeliveryArea', RESEED, 0);
                        DELETE FROM OrganisationStandard;
                        DBCC CHECKIDENT('OrganisationStandard', RESEED, 0);
                        DELETE FROM DeliveryArea;
                        DBCC CHECKIDENT('DeliveryArea', RESEED, 0);
                        DELETE FROM Options;
                        DELETE FROM StandardCollation;
                        DELETE FROM Contacts;
                        DELETE FROM Organisations;
                        DELETE FROM OrganisationType;
                        DBCC CHECKIDENT('OrganisationType', RESEED, 0);
                    ",
                    sourceConnection);

                var rowsAffected = teardownCommand.ExecuteNonQuery();
            }
        }

        public void Step1_Organisation_Data()
        {
            _aggregateLogger.LogInfo("Step 1: Syncing Organisation Data");
            BulkCopyData("SELECT * FROM dbo.OrganisationType", "dbo.OrganisationType");
            BulkCopyData("SELECT * FROM dbo.Organisations", "dbo.Organisations");
        }

        public void Step2_Contacts_Data()
        {
            _aggregateLogger.LogInfo("Step 2: Syncing Contacts");
            BulkCopyData("SELECT * FROM dbo.Contacts", "dbo.Contacts");
        }

        public void Step3_Standard_Data()
        {
            _aggregateLogger.LogInfo("Step 3: Syncing Standard Data");
            BulkCopyData("SELECT * FROM dbo.StandardCollation", "dbo.StandardCollation");
            BulkCopyData("SELECT * FROM dbo.Options", "dbo.Options");
        }

        public void Step4_OrganisationStandard_Data()
        {
            _aggregateLogger.LogInfo("Step 4: Syncing Organisation Standard Data");
            BulkCopyData("SELECT * FROM dbo.DeliveryArea", "dbo.DeliveryArea");
            BulkCopyData("SELECT * FROM dbo.OrganisationStandard", "dbo.OrganisationStandard");
            BulkCopyData("SELECT * FROM dbo.OrganisationStandardDeliveryArea", "dbo.OrganisationStandardDeliveryArea");
        }

        public void Step5_Obfuscate_Personal_Data()
        {
            _aggregateLogger.LogInfo("Step 5: Obfuscating Personal Data");
            using (var sourceConnection = new SqlConnection(_config.SqlConnectionString))
            {
                sourceConnection.Open();

                var obfuscateCommand = new SqlCommand(
                    @"  UPDATE Organisations
                        SET PrimaryContact = NULL
                            , ApiUser = NULL;

                        UPDATE Contacts
                        SET GivenNames = 'TEST'
                            , FamilyName = 'TEST'
                            , DisplayName = 'TEST TEST'
                            , Email = 'TEST@TEST.TEST'
                            , PhoneNumber = NULL
                            , SignInId = NULL;
                    ",
                    sourceConnection);

                var rowsAffected = obfuscateCommand.ExecuteNonQuery();
            }
        }

        public void Step6_Generate_Test_Data()
        {
            _aggregateLogger.LogInfo("Step 6: Generating Test Data");
            using (var sourceConnection = new SqlConnection(_config.SqlConnectionString))
            {
                sourceConnection.Open();

                var generateDataCommand = new SqlCommand(
                    @"  WITH CTE AS (
                          SELECT 0 as Number
                          UNION ALL
                          SELECT Number+1
                          FROM CTE 
                          WHERE Number < 9 
                        )
                        INSERT INTO [Ilrs](Id, CreatedAt, Uln, FamilyName ,GivenNames, UkPrn, StdCode, LearnStartDate, EpaOrgId, FundingModel, Apprenticeshipid, EmployerAccountId, Source, LearnRefNumber, CompletionStatus, EventId, PlannedEndDate)
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
                          NULL AS Apprenticeshipid,
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
                          CROSS JOIN organisationstandard ogs 
                          JOIN Organisations og1 ON og1.EndPointAssessorOrganisationId = ogs.EndPointAssessorOrganisationId AND og1.Status <> 'Deleted'
                          JOIN StandardCollation sc1 ON ogs.StandardCode = sc1.StandardId
                        WHERE  ogs.Status NOT IN ( 'Deleted','New') AND (ogs.EffectiveTo IS NULL OR ogs.EffectiveTo > GETDATE()) AND og1.EndPointAssessorUkprn IS NOT NULL
                        ) ab1
                        ORDER BY Uln, EndPointAssessorOrganisationId, StandardCode, Number
                    ",
                    sourceConnection);

                var rowsAffected = generateDataCommand.ExecuteNonQuery();
            }
        }

        private void BulkCopyData(string sqlCommandText, string destinationTableName)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlbulkcopy

            if (string.IsNullOrWhiteSpace(sqlCommandText)) throw new ArgumentNullException(nameof(sqlCommandText));
            if (string.IsNullOrWhiteSpace(destinationTableName)) throw new ArgumentNullException(nameof(destinationTableName));

            string ppDatabase = null; // TODO: This needs to be a new connection string that is within the CONFIG SETTINGS
            using (var sourceConnection = new SqlConnection(ppDatabase))
            {
                sourceConnection.Open();

                var commandSourceData = new SqlCommand(sqlCommandText, sourceConnection);

                using (var reader = commandSourceData.ExecuteReader())
                {
                    using (var destinationConnection = new SqlConnection(_config.SqlConnectionString))
                    {
                        destinationConnection.Open();

                        using (var bulkCopy = new SqlBulkCopy(destinationConnection))
                        {
                            bulkCopy.DestinationTableName = destinationTableName;
                            bulkCopy.WriteToServer(reader);
                        }
                    }
                }
            }
        }
    }
}
