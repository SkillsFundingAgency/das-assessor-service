using System;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class EpaoImporterCommand : ICommand
    {
        private readonly IEaoImporterLogger _eaoImporterLogger;
        private readonly TokenService _tokenService;

        public EpaoImporterCommand(IEaoImporterLogger eaoImporterLogger,
            TokenService tokenService)
        {
            _eaoImporterLogger = eaoImporterLogger;
            _tokenService = tokenService;
        }

        public async Task Execute()
        {
            try
            {
                _eaoImporterLogger.LogInfo("EAO Importer Function Started");

                var webConfig = ConfigurationHelper.GetConfiguration();

                _eaoImporterLogger.LogInfo("Config Received");

                var token = _tokenService.GetToken();

                _eaoImporterLogger.LogInfo("Token Received");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept", "application/json");

                    var response = await client.PostAsync($"{webConfig.ClientApiAuthentication.ApiBaseAddress}/api/v1/register-import", new StringContent(""));
                    var content = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        _eaoImporterLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {content}");
                    }
                    else
                    {
                        _eaoImporterLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {content}");
                    }
                }
            }
            catch (Exception e)
            {
                _eaoImporterLogger.LogError("Function Errored", e);
                throw;
            }
        }
    }
}
