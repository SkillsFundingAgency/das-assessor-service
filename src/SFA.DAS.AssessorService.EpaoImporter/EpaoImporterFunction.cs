using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using SFA.DAS.AssessorService.EpaoImporter.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class EpaoImporterFunction
    {
        private static TraceWriter _functionLogger;
        private static Logger _redisLogger;

        [FunctionName("EpaoImporterFunction")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter functionLogger, ExecutionContext context)
        {
            try
            {
                _functionLogger = functionLogger;

                LogManager.Configuration = new XmlLoggingConfiguration($@"{context.FunctionAppDirectory}\nlog.config");
                _redisLogger = LogManager.GetCurrentClassLogger();

                LogInfo("Function Started");

                var webConfig = GetConfig();

                LogInfo("Config Received");

                var token = GetToken(webConfig);

                LogInfo("Token Received");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept", "application/json");

                    var response = client.PostAsync($"{webConfig.ClientApiAuthentication.ApiBaseAddress}/api/v1/register-import", new StringContent("")).Result;
                    var content = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        LogInfo($"Status code returned: {response.StatusCode}. Content: {content}");
                    }
                    else
                    {
                        LogInfo($"Status code returned: {response.StatusCode}. Content: {content}");
                    }
                }
            }
            catch (Exception e)
            {
                LogError("Function Errored", e);
                throw;
            }
        }

        private static void LogError(string message, Exception ex)
        {
            _functionLogger.Error(message,ex);
            _redisLogger.Error(ex, message);
        }

        private static void LogInfo(string message)
        {
            _functionLogger.Info(message);
            _redisLogger.Info(message);
        }

        private static string GetToken(WebConfiguration webConfig)
        {
            var tenantId = webConfig.ClientApiAuthentication.TenantId;
            var clientId = webConfig.ClientApiAuthentication.ClientId;
            var appKey = webConfig.ClientApiAuthentication.ClientSecret;
            var resourceId = webConfig.ClientApiAuthentication.ResourceId;

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var tokenResult = context.AcquireTokenAsync(resourceId, clientCredential).Result;
            return tokenResult.AccessToken;
        }

        private static WebConfiguration GetConfig()
        {
            string ServiceName = "SFA.DAS.AssessorService";
            string Version = "1.0";

            //var configuration = ConfigurationService
            //        .GetConfig(Environment.GetEnvironmentVariable("EnvironmentName", EnvironmentVariableTarget.Process),
            //        Environment.GetEnvironmentVariable("Storage", EnvironmentVariableTarget.Process), Version, ServiceName).Result;

            var conn = CloudStorageAccount.Parse(
                Environment.GetEnvironmentVariable("ConfigurationStorageConnectionString", EnvironmentVariableTarget.Process));
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
