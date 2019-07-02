using Microsoft.Azure.WebJobs;
using SFA.DAS.AssessorService.EpaoImporter.Startup;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class PrintFunctionFlow
    {
        [FunctionName("PrintFunctionFlow")]
        public static void Run([TimerTrigger("0 */3 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger functionLogger,
            ExecutionContext context)
        {
            new Bootstrapper().StartUp(functionLogger, context);

            var command = Bootstrapper.Container.GetInstance<PrintProcessCommand>();
            command.Execute().GetAwaiter().GetResult();
        }
    }
}

