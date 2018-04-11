using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Renci.SshNet;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.InfrastructureServices;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
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


            INotificationsApiClientConfiguration clientConfiguration = new NotificationsApiClientConfiguration
            {
                ApiBaseUrl = "https://at-notifications.apprenticeships.sfa.bis.gov.uk/",
                ClientToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJkYXRhIjoiU2VuZFNtcyBTZW5kRW1haWwiLCJpc3MiOiJodHRwOi8vZGFzLWF0LW5vdC1jcyIsImF1ZCI6Imh0dHA6Ly9kYXMtYXQtZWFzLWNzIiwiZXhwIjoxNTU1NzU2OTQ4LCJuYmYiOjE1MjMzNTY5NDh9.2IKr0p-nq5KscucjgzhXrfbVQ_mdQ63yH3PLSVSZ9Xk",
                ClientId = "",
                ClientSecret = "",
                IdentifierUri = "",
                Tenant = ""
            };


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

                var notificationHttpClient = new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(clientConfiguration)).Build();
                configure.For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(notificationHttpClient);
                configure.For<INotificationsApiClientConfiguration>().Use(clientConfiguration);

            });
        }

        public static Container Container { get; private set; }
    }
}
