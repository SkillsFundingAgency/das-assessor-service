using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices
{
    public static class ConfigurationHelper
    {

        private static IWebConfiguration _webConfiguration;

        public static IWebConfiguration GetConfiguration()
        {
            if (_webConfiguration == null)
            {
                var ServiceName = "SFA.DAS.AssessorService";
                var Version = "1.0";

                var conn = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
                var tableClient = conn.CreateCloudTableClient();
                var table = tableClient.GetTableReference("Configuration");

                var operation = TableOperation.Retrieve(
                    CloudConfigurationManager.GetSetting("EnvironmentName"),
                    $"{ServiceName}_{Version}");
                var result = table.ExecuteAsync(operation).Result;

                var dynResult = result.Result as DynamicTableEntity;
                var data = dynResult.Properties["Data"].StringValue;

                _webConfiguration = JsonConvert.DeserializeObject<WebConfiguration>(data);
            }

            return _webConfiguration;
        }
    }
}