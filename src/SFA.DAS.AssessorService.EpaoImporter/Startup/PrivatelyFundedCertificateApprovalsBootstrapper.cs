using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoImporter.Const;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Startup.DependencyResolution;
using SFA.DAS.AssessorService.Settings;
using StructureMap;

namespace SFA.DAS.AssessorService.EpaoImporter.Startup
{
    public class PrivatelyFundedCertificateApprovalsBootstrapper
    {
        private readonly Container _container;

        public PrivatelyFundedCertificateApprovalsBootstrapper(TraceWriter functionLogger, ExecutionContext context)
        {
            IAggregateLogger logger = new AggregateLogger(FunctionName.PrivatelyFundedCertificateApprovals, functionLogger, context);

            var configuration = ConfigurationHelper.GetConfiguration();

            logger.LogInfo("Initialising bootstrapper ....");
            logger.LogInfo("Config Received");

            _container = new Container(configure =>
            {
                configure.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });

                configure.For<IAggregateLogger>().Use(logger).Singleton();
                configure.For<IWebConfiguration>().Use(configuration).Singleton();

                //configure.For<IFileTransferClient>().Use<FileTransferClient>();
                //configure.For<IAssessorServiceApi>().Use<AssessorServiceApi>().Singleton();
                //configure.For<INotificationService>().Use<NotificationService>();
                //configure.For<SftpClient>().Use<SftpClient>("SftpClient",
                //    c => new SftpClient(configuration.Sftp.RemoteHost, Convert.ToInt32(configuration.Sftp.Port),
                //        configuration.Sftp.Username, configuration.Sftp.Password));
                //configure.AddRegistry<NotificationsRegistry>();

                logger.LogInfo("Calling http registry and getting the token ....");
                configure.AddRegistry<HttpRegistry>();
            });

            var language = "en-GB";
            System.Threading.Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo(language);
        }

        public T GetInstance<T>()
        {
            return _container.GetInstance<T>();
        }        
    }
}
