using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoImporter.Const;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class EpaoImporterFunction
    {
        [FunctionName("EpaoImporterFunction")]
        public static void Run([TimerTrigger("0 0 2 * * *")] TimerInfo myTimer, TraceWriter functionLogger,
            ExecutionContext context)
        {

            var _epaoImporterLogger = new AggregateLogger(FunctionName.EpaoImporter, functionLogger, context);
            try
            {
				_epaoImporterLogger.LogInfo("EAO Importer Function Disabled");
/*
				_epaoImporterLogger.LogInfo("EAO Importer Function Started");

                var webConfig = ConfigurationHelper.GetConfiguration();
                var _tokenService = new TokenService(webConfig);

                _epaoImporterLogger.LogInfo("Config Received");

                var token = _tokenService.GetToken();

                _epaoImporterLogger.LogInfo("Token Received");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept", "application/json");

                    var response = client.PostAsync($"{webConfig.ClientApiAuthentication.ApiBaseAddress}/api/v1/register-import", new StringContent("")).Result;
                    var content = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        _epaoImporterLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {content}");
                    }
                    else
                    {
                        _epaoImporterLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {content}");
                    }
                }
*/
            }
            catch (Exception e)
            {
                _epaoImporterLogger.LogError("Function Errored", e);
                throw;
            }
        }
    }
}
