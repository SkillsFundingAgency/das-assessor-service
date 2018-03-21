using Microsoft.Extensions.Configuration;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class AppMain
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
