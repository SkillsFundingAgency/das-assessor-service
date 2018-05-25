using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoImporter.Startup;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class EpaoImporterFunction
    {
        [FunctionName("EpaoImporterFunction")]
        public static void Run([TimerTrigger("0 15 * * * *"), Disable()] TimerInfo myTimer, TraceWriter functionLogger,
            ExecutionContext context)
        {
            new Bootstrapper().StartUp(functionLogger, context);

            var command = Bootstrapper.Container.GetInstance<EpaoImporterCommand>();
            command.Execute().GetAwaiter().GetResult();
        }
    }
}
