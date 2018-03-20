using System;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class AppMain
    {
        static void Main(string[] args)
        {
            Bootstrapper.Initialise();

            var command = Bootstrapper.Container.GetInstance<Command>();
            command.Execute().GetAwaiter().GetResult();
        }
    }
}
