using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class StandardCollationCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IAssessorServiceApi _assessorServiceApi;

        public StandardCollationCommand(IAggregateLogger aggregateLogger, IAssessorServiceApi assessorServiceApi)
        {
            _aggregateLogger = aggregateLogger;
            _assessorServiceApi = assessorServiceApi;
        }

        public async Task Execute()
        {
            _aggregateLogger.LogInfo("Standard Collation Gathering Function Started");
            _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");
            await _assessorServiceApi.GatherStandards();    
        }
    }
}
