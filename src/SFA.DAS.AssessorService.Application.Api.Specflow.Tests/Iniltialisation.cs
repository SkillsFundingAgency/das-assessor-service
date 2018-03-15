using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    using TechTalk.SpecFlow;

    [Binding]
    public class Initialisation
    {
        [BeforeTestRun]
        public static void Setup()
        {
            Bootstrapper.Initialise();

            Mapper.Initialize(cfg =>
            {
              
            });
        }
    }
}
