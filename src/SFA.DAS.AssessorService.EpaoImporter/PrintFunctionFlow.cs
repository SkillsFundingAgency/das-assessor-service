using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoImporter.Const;
using SFA.DAS.AssessorService.EpaoImporter.Startup;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class PrintFunctionFlow
    {
        [FunctionName("PrintFunctionFlow")]
        //public static void Run([TimerTrigger("0 0,30 * * * *",
                   public static void Run([TimerTrigger("0 */3 * * * *",
                RunOnStartup = false)] TimerInfo myTimer, TraceWriter functionLogger,
            ExecutionContext context)
        {

            new Bootstrapper().StartUp(FunctionName.PrintProcessFlow, functionLogger, context);

            var command = Bootstrapper.Container.GetInstance<PrintProcessFlowCommand>();
            command.Execute().GetAwaiter().GetResult();
        }
    }
}
