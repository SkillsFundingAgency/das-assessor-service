using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoDataSync.Data;
using SFA.DAS.AssessorService.EpaoDataSync.Domain;
using SFA.DAS.AssessorService.EpaoDataSync.Infrastructure;
using SFA.DAS.AssessorService.EpaoDataSync.Logger;
using SFA.DAS.AssessorService.EpaoDataSync.Startup.DependencyResolution;
using StructureMap;

namespace SFA.DAS.AssessorService.EpaoDataSync.Startup
{
    public static class Bootstrapper
    {
        private static IAggregateLogger _logger;

        public static void StartUp(TraceWriter functionLogger, ExecutionContext context)
        {
            var configuration = ConfigurationHelper.GetConfiguration();

            _logger = new AggregateLogger("das-assessor-service-func-epodatasync", functionLogger, context);
           
            _logger.LogInfo("Initialising bootstrapper ....");
            _logger.LogInfo("Config Received");

            Container = new Container(configure =>
            {
                configure.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });

                configure.For<IAggregateLogger>().Use(_logger).Singleton();
                configure.For<IWebConfiguration>().Use(configuration).Singleton();
                configure.For<IProviderEventServiceApi>().Use<ProviderEventsServiceApi>().Singleton();
                configure.For<IDbConnection>().Use(c => new SqlConnection(configuration.SqlConnectionString));
                configure.For<IIlrsRefresherService>().Use<IlrsRefresherService>().Singleton();
                configure.AddRegistry<HttpRegistry>();
            });

            var language = "en-GB";
            System.Threading.Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo(language);
        }

        public static Container Container { get; private set; }
    }
}
