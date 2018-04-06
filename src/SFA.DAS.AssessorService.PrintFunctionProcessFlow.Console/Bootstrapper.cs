using System;
using System.Data;
using System.Data.SqlClient;
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

            var localConfiguration = builder.Build();

            Configuration = ConfigurationService
                .GetConfig(localConfiguration["EnvironmentName"], localConfiguration["Values:ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            var connectionString = Configuration.SqlConnectionString;
            var sqlConnection = new SqlConnection(connectionString);
        
            Container = new Container(configure =>
            {
                configure.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });

                configure.For<IConfiguration>().Use(localConfiguration);
                configure.For<SftpClient>().Use<SftpClient>("Build ISession from ISessionFactory",
                    c => new SftpClient(Configuration.Sftp.RemoteHost, Convert.ToInt32(Configuration.Sftp.Port), Configuration.Sftp.Username, Configuration.Sftp.Password));
                configure.For<IDbConnection>().Use<SqlConnection>(sqlConnection);
            });
        }

        public static Container Container { get; private set; }
    }
}
