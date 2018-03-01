using System;
using System.Linq;
using System.Reflection;
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
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            RedirectAssembly();

            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            var baseUri = Environment.GetEnvironmentVariable("ApiBaseAddress", EnvironmentVariableTarget.Process);

            var tokenService = new TokenService(
                new WebConfiguration()
                {
                    ClientApiAuthentication = new ClientApiAuthentication()
                    {
                        ClientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process),
                        Domain = Environment.GetEnvironmentVariable("Domain", EnvironmentVariableTarget.Process),
                        ClientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process),
                        ResourceId = Environment.GetEnvironmentVariable("ResourceId", EnvironmentVariableTarget.Process)
                    }
                });
            
            using (var apiClient = new RegisterImportApiClient(baseUri, tokenService))
            {
                apiClient.Import().Wait();
            }
        }

        public static void RedirectAssembly()
        {
            var list = AppDomain.CurrentDomain.GetAssemblies().OrderByDescending(a => a.FullName).Select(a => a.FullName).ToList();
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                foreach (string asmName in list)
                {
                    if (asmName.StartsWith(requestedAssembly.Name + ","))
                    {
                        return Assembly.Load(asmName);
                    }
                }
                return null;
            };
        }
    }
}