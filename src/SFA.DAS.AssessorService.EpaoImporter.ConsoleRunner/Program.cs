using System;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiBaseUrl = "http://localhost:59021";

            var tokenService = new TokenService(new InMemoryCache(),
                new WebConfiguration()
                {
                    Api = new ApiSettings()
                    {
                        TokenEncodingKey = "Wt+69DPlA9wjXl79V9N67bR4cpn9+3zZmgLJHBXy2aQ=",
                        ApiBaseAddress = "http://localhost:59021"
                    }
                });

            using (var apiClient = new RegisterImportApiClient(apiBaseUrl, tokenService))
            {
                apiClient.Import("IMPORTER").Wait();
            }

            Console.ReadKey();
        }
    }
}
