﻿using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Renci.SshNet;
using SFA.DAS.AssessorService.Functions.Data;
using SFA.DAS.AssessorService.Functions.InfrastructureServices;
using SFA.DAS.AssessorService.Functions.Logger;
using SFA.DAS.AssessorService.Functions.Settings;
using SFA.DAS.AssessorService.Functions.Startup.DependencyResolution;
using StructureMap;

namespace SFA.DAS.AssessorService.Functions.Startup
{
    public class Bootstrapper
    {
        private IAggregateLogger _logger;

        public void StartUp(string source, TraceWriter functionLogger, ExecutionContext context)
        {
            _logger = new AggregateLogger(source, functionLogger, context);

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
