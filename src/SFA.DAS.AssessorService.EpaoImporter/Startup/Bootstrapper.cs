using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Renci.SshNet;
using SFA.DAS.AssessorService.EpaoImporter.Const;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;
using SFA.DAS.AssessorService.EpaoImporter.Startup.DependencyResolution;
using SFA.DAS.AssessorService.Settings;
using StructureMap;

namespace SFA.DAS.AssessorService.EpaoImporter.Startup
{
    public class Bootstrapper
    {
        private IAggregateLogger _logger;            

        public void StartUp(TraceWriter functionLogger, ExecutionContext context)
        {

            _logger = new AggregateLogger(FunctionName.PrintProcessFlow, functionLogger, context);          

            var configuration = ConfigurationHelper.GetConfiguration();

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

                configure.For<IFileTransferClient>().Use<FileTransferClient>();
                configure.For<IAssessorServiceApi>().Use<AssessorServiceApi>().Singleton();
                configure.For<INotificationService>().Use<NotificationService>();
                configure.For<SftpClient>().Use<SftpClient>("SftpClient",
                    c => new SftpClient(configuration.Sftp.RemoteHost, Convert.ToInt32(configuration.Sftp.Port),
                        configuration.Sftp.Username, configuration.Sftp.Password));
                configure.AddRegistry<NotificationsRegistry>();

                _logger.LogInfo("Calling http registry and getting the token ....");
                configure.AddRegistry<HttpRegistry>();
            });

            var language = "en-GB";
            System.Threading.Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo(language);
        }

        public static Container Container { get; private set; }
    }
}
