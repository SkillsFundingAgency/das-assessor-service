using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoImporter.Startup;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class PrivatelyFundedCertificatesApprovalNotificationService
    {
        [FunctionName("PrivatelyFundedCertificatesApprovalNotificationService")]
        public static void Run([TimerTrigger("0 0 10 * * 3")] TimerInfo myTimer, TraceWriter functionLogger,
            ExecutionContext context)
        {
            var privatelyFundedCertificateApprovalsBootstrapper = new PrivatelyFundedCertificateApprovalsBootstrapper(functionLogger, context);

            var command = privatelyFundedCertificateApprovalsBootstrapper.GetInstance<PrivatelyFundedCertificatesApprovalNotificationCommand>();
            command.Execute().GetAwaiter().GetResult();
        }
    }
}

