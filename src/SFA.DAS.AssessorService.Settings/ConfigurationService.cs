using System;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
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

        private static async Task<T> GetConfig<T>(string environment, string storageConnectionString, string version, string serviceName)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            if (storageConnectionString == null) throw new ArgumentNullException(nameof(storageConnectionString));

            var tableServiceClient = new TableServiceClient(storageConnectionString);
            var tableClient = tableServiceClient.GetTableClient("Configuration");

            var rowKey = $"{serviceName}_{version}";
            try
            {
                var response = await tableClient.GetEntityAsync<TableEntity>(environment, rowKey);
                var data = response.Value.GetString("Data");
                var webConfig = JsonConvert.DeserializeObject<T>(data);
                return webConfig;
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                throw new Exception("The specified configuration was not found.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to Storage to retrieve settings.", e);
            }
        }
    }
}