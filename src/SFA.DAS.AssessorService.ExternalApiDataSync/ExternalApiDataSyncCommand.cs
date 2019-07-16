using SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure;
using SFA.DAS.AssessorService.ExternalApiDataSync.Logger;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.ExternalApiDataSync
{
    public interface ICommand
    {
        Task Execute();
    }

    public class ExternalApiDataSyncCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly bool _allowDataSync;
        private readonly string _sourceConnectionString;
        private readonly string _destinationConnectionString;

        public ExternalApiDataSyncCommand(IWebConfiguration config, IAggregateLogger aggregateLogger)
        {
            _aggregateLogger = aggregateLogger;

            _allowDataSync = config.ExternalApiDataSync.IsEnabled;
            _sourceConnectionString = config.SqlConnectionString;
            _destinationConnectionString = config.SandboxSqlConnectionString;
        }

        public async Task Execute()
        {
            _aggregateLogger.LogInfo("External Api Data Sync Function Started");
            _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

            if (_allowDataSync)
            {
                _aggregateLogger.LogInfo("Proceeding with External Api Data Sync...");
            }
            else
            {
                _aggregateLogger.LogInfo("External Api Data Sync is disabled at this time");
            }
        }
    }
}
