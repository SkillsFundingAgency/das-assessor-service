using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoImporter.Startup;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class PrivatelyFundedCertificatesUddateStatusService
    {
        [FunctionName("PrivatelyFundedCertificatesUpdateStatusService")]
        public static void Run([TimerTrigger("0 30 18 * * 4", RunOnStartup = true)]
            TimerInfo myTimer, TraceWriter functionLogger,
            ExecutionContext context)
        {
            var privatelyFundedCertificateApprovalsBootstrapper =
                new PrivatelyFundedCertificateApprovalsBootstrapper(functionLogger, context);

            var command = privatelyFundedCertificateApprovalsBootstrapper
                .GetInstance<PrivatelyFundedCertificatesUpdateStatusCommand>();
            command.Execute().GetAwaiter().GetResult();
        }
    }
}