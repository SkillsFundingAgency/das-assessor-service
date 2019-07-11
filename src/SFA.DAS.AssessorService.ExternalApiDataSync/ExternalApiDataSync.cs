using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.ExternalApiDataSync.Startup;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.ExternalApiDataSync
{
    public static class ExternalApiDataSync
    {
        [FunctionName("ExternalApiDataSync")]
        public static async Task Run([TimerTrigger("0 0 0 1 * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"ExternalApiDataSync function executed at: {DateTime.Now}");

            Bootstrapper.StartUp(log, context);
            var command = Bootstrapper.Container.GetInstance<ExternalApiDataSyncCommand>();
            await command.Execute();
        }
    }
}
