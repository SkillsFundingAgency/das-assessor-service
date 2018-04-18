using System;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class EpaoImporterCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly TokenService _tokenService;

        public EpaoImporterCommand(IAggregateLogger aggregateLogger,
            TokenService tokenService)
        {
            _aggregateLogger = aggregateLogger;
            _tokenService = tokenService;
        }

        public async Task Execute()
        {
            try
            {
                _aggregateLogger.LogInfo("Function Started");

                var webConfig = ConfigurationHelper.GetConfiguration();

                _aggregateLogger.LogInfo("Config Received");

                var token = _tokenService.GetToken();

                _aggregateLogger.LogInfo("Token Received");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept", "application/json");

                    var response = await client.PostAsync($"{webConfig.ClientApiAuthentication.ApiBaseAddress}/api/v1/register-import", new StringContent(""));
                    var content = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        _aggregateLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {content}");
                    }
                    else
                    {
                        _aggregateLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {content}");
                    }
                }
            }
            catch (Exception e)
            {
                _aggregateLogger.LogError("Function Errored", e);
                throw;
            }
        }
    }
}
