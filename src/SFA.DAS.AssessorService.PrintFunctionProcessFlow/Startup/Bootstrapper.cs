using System;
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

            var configuration = ConfigurationHelper.GetConfiguration();
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
               
                configure.For<ICertificatesRepository>().Use<CertificatesRepository>().Singleton();
                configure.For<SftpClient>().Use<SftpClient>("SftpClient",
                    c => new SftpClient(configuration.Sftp.RemoteHost, Convert.ToInt32(configuration.Sftp.Port), configuration.Sftp.Username, configuration.Sftp.Password));

                configure.AddRegistry<NotificationsRegistry>();
                configure.AddRegistry<HttpRegistry>();
            });
        }

        public static Container Container { get; private set; }
    }
}
