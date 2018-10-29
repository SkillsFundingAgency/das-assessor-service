using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class PrivatelyFundedCertificatesUddateStatusCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IAssessorServiceApi _assessorServiceApi;
        private readonly IPrivatelyFundedCertificatesApprovalNotification _notificationService;

        public PrivatelyFundedCertificatesUddateStatusCommand(IAggregateLogger aggregateLogger,
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
                _aggregateLogger.LogInfo("Privately Funded Certificate Approval Notification Started");
                _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

//                await _assessorServiceApi.UpdatePrivatelyFundedCertificateRequestsToBeApproved();
//
//                var certificatesToBeApproved = await _assessorServiceApi.GetCertificatesToBeApproved();
//                await _notificationService.Send(certificatesToBeApproved);

            }
            catch (Exception e)
            {
                _aggregateLogger.LogError("Function Errored", e);
                throw;
            }
        }
    }
}
