using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoImporter.Startup;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class StandardCollationFlow
    {
        [FunctionName("StandardCollationFlow")]
        public static void Run([TimerTrigger("0 30 10 * * *", RunOnStartup = true)] TimerInfo myTimer, TraceWriter functionLogger,
            ExecutionContext context)
        {

            new Bootstrapper().StartUp(functionLogger, context);

            var command = Bootstrapper.Container.GetInstance<StandardCollationCommand>();
            command.Execute().GetAwaiter().GetResult();
        }
    }
}
