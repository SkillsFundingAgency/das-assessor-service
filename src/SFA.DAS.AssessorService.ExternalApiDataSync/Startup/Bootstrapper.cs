using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.ExternalApiDataSync.Helpers;
using SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure;
using SFA.DAS.AssessorService.ExternalApiDataSync.Logger;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Startup
{
    public static class Bootstrapper
    {
        private static IAggregateLogger _logger;
        private static AzureServiceTokenProvider _azureServiceTokenProvider;

        public static void StartUp(ILogger functionLogger, ExecutionContext context)
        {
            _logger = new AggregateLogger("das-assessor-service-func-externalapidatasync", functionLogger, context);

            var configuration = ConfigurationHelper.GetConfiguration();

            _logger.LogInformation("Initialising bootstrapper ....");
            _logger.LogInformation("Config Received");

            var currentEnvironment = Environment.GetEnvironmentVariable("EnvironmentName");

            if (!currentEnvironment.Equals("DEV", StringComparison.CurrentCultureIgnoreCase) && !currentEnvironment.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                _azureServiceTokenProvider = new AzureServiceTokenProvider();
            }

            Container = new Container(configure =>
            {
                configure.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });

                configure.For<IAggregateLogger>().Use(_logger).Singleton();
                configure.For<IWebConfiguration>().Use(configuration).Singleton();
                configure.For<IDbConnection>().Use(c => ManagedIdentitySqlConnection.GetSqlConnection(configuration.SqlConnectionString, _azureServiceTokenProvider));
            });

            var language = "en-GB";
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(language);
        }

        public static Container Container { get; private set; }
    }
}