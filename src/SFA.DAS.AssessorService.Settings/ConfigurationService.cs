using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public static class ConfigurationService
    {
        public static async Task<IApiConfiguration> GetConfigApi(string environment, string storageConnectionString, string version, string serviceName)
        {
            var config = await GetConfig<ApiConfiguration>(environment, storageConnectionString, version, serviceName);
            config.Environment = environment;
            return config;
        }

        public static async Task<IExternalApiConfiguration> GetConfigExternalApi(string environment, string storageConnectionString, string version, string serviceName)
        {
            var config = await GetConfig<ExternalApiConfiguration>(environment, storageConnectionString, version, serviceName);
            return config;
        }

        public static async Task<IWebConfiguration> GetConfigWeb(string environment, string storageConnectionString, string version, string serviceName)
        {
            var config = await GetConfig <WebConfiguration>(environment, storageConnectionString, version, serviceName);
            return config;
        }

        private static async Task<T> GetConfig<T>(string environment, string storageConnectionString, string version, string serviceName)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            if (storageConnectionString == null) throw new ArgumentNullException(nameof(storageConnectionString));

            var conn = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = conn.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Configuration");

            var operation = TableOperation.Retrieve(environment, $"{serviceName}_{version}");
            TableResult result;
            try
            {
                result = await table.ExecuteAsync(operation);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to Storage to retrieve settings.", e);
            }

            var dynResult = result.Result as DynamicTableEntity;
            var data = dynResult.Properties["Data"].StringValue;

            var webConfig = JsonConvert.DeserializeObject<T>(data);
            return webConfig;
        }
    }
}