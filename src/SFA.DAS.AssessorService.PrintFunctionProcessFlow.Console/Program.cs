using Microsoft.Extensions.Configuration;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Console
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            Bootstrapper.Initialise();

            var command = Bootstrapper.Container.GetInstance<Command>();
            command.Execute().GetAwaiter().GetResult();
        }
    }
}
