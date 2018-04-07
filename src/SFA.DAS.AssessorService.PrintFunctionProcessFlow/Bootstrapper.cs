using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Renci.SshNet;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.InfrastructureServices;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings;
using StructureMap;
//using Microsoft.Extensions.Configuration;
//using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public class Bootstrapper
    {
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";

        public IWebConfiguration Configuration { get; set; }

        public void StartUp(TraceWriter functionLogger, ExecutionContext context)
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("local.settings.json");

            //var localConfiguration = builder.Build();

            //Configuration = ConfigurationService
            //    .GetConfig(localConfiguration["EnvironmentName"], localConfiguration["Values:ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            //var connectionString = Configuration.SqlConnectionString;
            //var sqlConnection = new SqlConnection(connectionString);

            var agregateLogger = new AggregateLogger(functionLogger, context);
            var webConfiguration = ConfigurationService.GetConfiguration();

            agregateLogger.LogInfo("Config Received");

            var tokenService = new TokenService(webConfiguration);
            var token = tokenService.GetToken();

            agregateLogger.LogInfo($"Token Received => {token}");


            Container = new Container(configure =>
            {
                configure.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });

                configure.For<IAggregateLogger>().Use(agregateLogger).Singleton();
                configure.For<IWebConfiguration>().Use(webConfiguration).Singleton();
                configure.For<SftpClient>().Use<SftpClient>("Build ISession from ISessionFactory",
                    c => new SftpClient(Configuration.Sftp.RemoteHost, Convert.ToInt32(Configuration.Sftp.Port), Configuration.Sftp.Username, Configuration.Sftp.Password));
                //configure.For<IDbConnection>().Use<SqlConnection>(sqlConnection);
            });
        }

        public static Container Container { get; private set; }
    }
}
