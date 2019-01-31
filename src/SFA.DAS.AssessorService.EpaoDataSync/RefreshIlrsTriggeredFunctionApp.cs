using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoDataSync.Domain;
using SFA.DAS.AssessorService.EpaoDataSync.Startup;

namespace SFA.DAS.AssessorService.EpaoDataSync
{
    public static class RefreshIlrsTriggeredFunctionApp
    {
        [FunctionName("RefreshIlrsFromProviderEventsTriggered")]
        public static async Task Run([TimerTrigger("0 0 0 * * *", RunOnStartup = true)] TimerInfo myTimer, TraceWriter functionLogger,
            ExecutionContext context)
        {
            functionLogger.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            Bootstrapper.StartUp(functionLogger, context);
            var ilrRefresherService = Bootstrapper.Container.GetInstance<IIlrsRefresherService>();
            await ilrRefresherService.UpdateIlRsTable();
        }
    }
}