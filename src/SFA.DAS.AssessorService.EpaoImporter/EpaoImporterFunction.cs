using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.EpaoImporter.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class EpaoImporterFunction
    {
        [FunctionName("EpaoImporterFunction")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            var webConfig = GetConfig();

            var token = GetToken(webConfig);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("accept", "application/json");

                var response = client.PostAsync($"{webConfig.ClientApiAuthentication.ApiBaseAddress}/api/v1/register-import", new StringContent("")).Result;
                var content = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    log.Info($"Status code returned: {response.StatusCode}. Content: {content}");
                }
                else
                {
                    log.Error($"Status code returned: {response.StatusCode}. Content: {content}");
                }
            }
        }

        private static string GetToken(WebConfiguration webConfig)
        {
            var tenant = webConfig.ClientApiAuthentication.Domain;
            var clientId = webConfig.ClientApiAuthentication.ClientId; 
            var appKey = webConfig.ClientApiAuthentication.ClientSecret; 
            var resourceId = webConfig.ClientApiAuthentication.ResourceId; 

            var authority = $"https://login.microsoftonline.com/{tenant}";
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
            //        .GetConfig(Environment.GetEnvironmentVariable("Environment", EnvironmentVariableTarget.Process),
            //        Environment.GetEnvironmentVariable("Storage", EnvironmentVariableTarget.Process), Version, ServiceName).Result;

            var conn = CloudStorageAccount.Parse(
                Environment.GetEnvironmentVariable("Storage", EnvironmentVariableTarget.Process));
            var tableClient = conn.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Configuration");

            var operation = TableOperation.Retrieve(
                Environment.GetEnvironmentVariable("Environment", EnvironmentVariableTarget.Process),
                $"{ServiceName}_{Version}");
            var result = table.ExecuteAsync(operation).Result;

            var dynResult = result.Result as DynamicTableEntity;
            var data = dynResult.Properties["Data"].StringValue;

            var webConfig = JsonConvert.DeserializeObject<WebConfiguration>(data);
            return webConfig;
        }
    }
}