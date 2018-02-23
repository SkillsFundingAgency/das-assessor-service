namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    using TechTalk.SpecFlow;

    [Binding]
    public class Initialisation
    {
        [BeforeTestRun]
        public static void Setip()
        {
            Bootstrapper.Initialise();
        }
    }
}
