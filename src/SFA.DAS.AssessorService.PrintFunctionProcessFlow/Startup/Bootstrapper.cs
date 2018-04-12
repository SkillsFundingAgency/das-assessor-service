using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Renci.SshNet;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.InfrastructureServices;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Startup.DependencyResolution;
using StructureMap;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Startup
{
    public class Bootstrapper
    {
        private IAggregateLogger _logger;

        public void StartUp(TraceWriter functionLogger, ExecutionContext context)
        {
            _logger = new AggregateLogger(functionLogger, context);

            var configuration = ConfigurationService.GetConfiguration();
            _logger.LogInfo("Config Received");

            var httpClient = GetHttpClient(configuration);

            Container = new Container(configure =>
            {
                configure.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });

                configure.For<IAggregateLogger>().Use(_logger).Singleton();
                configure.For<IWebConfiguration>().Use(configuration).Singleton();
                configure.For<HttpClient>().Use<HttpClient>(httpClient);
                configure.For<ICertificatesRepository>().Use<MockCertificatesRepository>().Singleton();
                configure.For<SftpClient>().Use<SftpClient>("SftpClient",
                    c => new SftpClient(configuration.Sftp.RemoteHost, Convert.ToInt32(configuration.Sftp.Port), configuration.Sftp.Username, configuration.Sftp.Password));

                configure.AddRegistry<NotificationsRegistry>();
            });
        }

        private static HttpClient GetHttpClient(IWebConfiguration configuration)
        {
            var tokenService = new TokenService(configuration);
            var token = tokenService.GetToken();

            var baseAddress = configuration.ClientApiAuthentication.ApiBaseAddress;

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        public static Container Container { get; private set; }
    }
}
