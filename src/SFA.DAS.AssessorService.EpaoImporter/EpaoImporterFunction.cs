using System;
using JWT;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class EpaoImporterFunction
    {
        [FunctionName("EpaoImporterFunction")]
        public static void Run([TimerTrigger("* */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            var tokenService = new TokenService(new InMemoryCache(),
                new WebConfiguration()
                {
                    Api = new ApiSettings()
                    {
                        TokenEncodingKey = Environment.GetEnvironmentVariable("TokenEncodingKey", EnvironmentVariableTarget.Process),
                        ApiBaseAddress = Environment.GetEnvironmentVariable("ApiBaseAddress", EnvironmentVariableTarget.Process)
                    }
                }, new UtcDateTimeProvider());

            using (var apiClient = new RegisterImportApiClient(Environment.GetEnvironmentVariable("ApiBaseAddress", EnvironmentVariableTarget.Process), tokenService))
            {
                apiClient.Import("IMPORTER").Wait();
            }
        }
    }
}