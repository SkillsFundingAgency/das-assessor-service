using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoImporter.Startup;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class ExternalApiDataSync
    {
        [FunctionName("ExternalApiDataSync")]
        public static void Run([TimerTrigger("0 0 0 1 1/1 *",RunOnStartup = true)] TimerInfo myTimer, TraceWriter functionLogger, ExecutionContext context)
        {
            var bootstrapper = new ExternalApiDataSyncBootstrapper(functionLogger, context);

            var command = bootstrapper.GetInstance<ExternalApiDataSyncCommand>();
            command.Execute().GetAwaiter().GetResult();
        }
    }
}
