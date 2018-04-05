using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using SFA.DAS.AssessorService.Settings;
using StructureMap;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Console
{
    public class Bootstrapper
    {
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";

        public IWebConfiguration Configuration { get; set; }

        public void Initialise()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json");

            var configuration = builder.Build();

            Configuration = ConfigurationService
                .GetConfig(configuration["EnvironmentName"], configuration["Values:ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            Container = new Container(configure =>
            {
                configure.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });

                configure.For<IConfiguration>().Use(configuration);
                configure.For<SftpClient>().Use<SftpClient>("Build ISession from ISessionFactory",
                    c => new SftpClient(Configuration.Sftp.RemoteHost, Convert.ToInt32(Configuration.Sftp.Port), Configuration.Sftp.Username, Configuration.Sftp.Password));
            });
        }

        public static Container Container { get; private set; }
    }
}
