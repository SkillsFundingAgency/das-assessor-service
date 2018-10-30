using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class PrivatelyFundedCertificatesUpdateStatusCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IAssessorServiceApi _assessorServiceApi;
        private readonly IPrivatelyFundedCertificatesApprovalNotification _notificationService;

        public PrivatelyFundedCertificatesUpdateStatusCommand(IAggregateLogger aggregateLogger,
            IAssessorServiceApi assessorServiceApi,
            IPrivatelyFundedCertificatesApprovalNotification notificationService
            )
        {
            _aggregateLogger = aggregateLogger;
            _assessorServiceApi = assessorServiceApi;
            _notificationService = notificationService;
        }

        public async Task Execute()
        {
            try
            {
                _aggregateLogger.LogInfo("Prepare Private Certificates for BatchRun Started");
                _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

                await _assessorServiceApi.PreparePrivateCertificatesforBatchRun();
            }
            catch (Exception e)
            {
                _aggregateLogger.LogError("Function Errored", e);
                throw;
            }
        }
    }
}
