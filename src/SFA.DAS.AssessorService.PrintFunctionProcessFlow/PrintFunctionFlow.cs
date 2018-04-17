using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Startup;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public static class PrintFunctionFlow
    {
        [FunctionName("PrintFunctionFlow")]
        public static void Run([TimerTrigger("0 */2 * * * *", 
                RunOnStartup = false)] TimerInfo myTimer, TraceWriter functionLogger,
            ExecutionContext context)
        {
            try
            {
                new Bootstrapper().StartUp(functionLogger, context);

                var command = Bootstrapper.Container.GetInstance<Command>();
                command.Execute().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                functionLogger.Error("Function Errored", e);
                throw;
            }
        }
    }
}