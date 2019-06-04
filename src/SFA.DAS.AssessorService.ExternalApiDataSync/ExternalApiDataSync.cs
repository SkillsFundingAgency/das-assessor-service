using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.ExternalApiDataSync.Startup;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.ExternalApiDataSync
{
    public static class ExternalApiDataSync
    {
        [FunctionName("ExternalApiDataSync")]
        public static async Task Run([TimerTrigger("0 0 0 1 1/1 *", RunOnStartup = true)] TimerInfo myTimer, TraceWriter functionLogger, ExecutionContext context)
        {
            functionLogger.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            Bootstrapper.StartUp(functionLogger, context);
            var command = Bootstrapper.Container.GetInstance<ExternalApiDataSyncCommand>();
            await command.Execute();
        }
    }
}
