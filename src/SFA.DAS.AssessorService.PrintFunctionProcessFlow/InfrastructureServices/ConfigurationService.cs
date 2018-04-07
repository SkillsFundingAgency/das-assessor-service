using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.InfrastructureServices
{
    public static class ConfigurationService
    {
        public static IWebConfiguration GetConfiguration()
        {
            var ServiceName = "SFA.DAS.AssessorService";
            var Version = "1.0";

            //var configuration = ConfigurationService
            //        .GetConfig(Environment.GetEnvironmentVariable("EnvironmentName", EnvironmentVariableTarget.Process),
            //        Environment.GetEnvironmentVariable("Storage", EnvironmentVariableTarget.Process), Version, ServiceName).Result;

            var conn = CloudStorageAccount.Parse(
                Environment.GetEnvironmentVariable("ConfigurationStorageConnectionString",
                    EnvironmentVariableTarget.Process));
            var tableClient = conn.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Configuration");

            var operation = TableOperation.Retrieve(
                Environment.GetEnvironmentVariable("EnvironmentName", EnvironmentVariableTarget.Process),
                $"{ServiceName}_{Version}");
            var result = table.ExecuteAsync(operation).Result;

            var dynResult = result.Result as DynamicTableEntity;
            var data = dynResult.Properties["Data"].StringValue;

            var webConfig = JsonConvert.DeserializeObject<WebConfiguration>(data);
            return webConfig;
        }
    }
}