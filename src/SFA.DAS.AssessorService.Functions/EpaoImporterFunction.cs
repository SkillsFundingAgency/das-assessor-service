using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.Functions.Const;
using SFA.DAS.AssessorService.Functions.Startup;

namespace SFA.DAS.AssessorService.Functions
{
    public static class EpaoImporterFunction
    {
        [FunctionName("EpaoImporterFunction")]
        public static void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, TraceWriter functionLogger,
            ExecutionContext context)
        {
            new Bootstrapper().StartUp(FunctionName.EpaoImporter, functionLogger, context);

            var command = Bootstrapper.Container.GetInstance<EpaoImporterCommand>();
            command.Execute().GetAwaiter().GetResult();
        }
    }
}
