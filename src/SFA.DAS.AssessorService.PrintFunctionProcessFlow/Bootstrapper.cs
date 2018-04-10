using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Renci.SshNet;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.InfrastructureServices;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings;
using StructureMap;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public class Bootstrapper
    {
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";

        public IWebConfiguration Configuration { get; set; }

        public void StartUp(TraceWriter functionLogger, ExecutionContext context)
        {
            var agregateLogger = new AggregateLogger(functionLogger, context);
            Configuration = ConfigurationService.GetConfiguration();

            agregateLogger.LogInfo("Config Received");

            var tokenService = new TokenService(Configuration);
            var token = tokenService.GetToken();

            var baseAddress = Configuration.ClientApiAuthentication.ApiBaseAddress;

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            agregateLogger.LogInfo($"Token Received => {token}");

            Container = new Container(configure =>
            {
                configure.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });

                configure.For<IAggregateLogger>().Use(agregateLogger).Singleton();
                configure.For<IWebConfiguration>().Use(Configuration).Singleton();
                configure.For<SftpClient>().Use<SftpClient>("SftpClient",
                    c => new SftpClient(Configuration.Sftp.RemoteHost, Convert.ToInt32(Configuration.Sftp.Port), Configuration.Sftp.Username, Configuration.Sftp.Password));
                configure.For<HttpClient>().Use<HttpClient>(httpClient);
            });
        }

        public static Container Container { get; private set; }
    }
}
