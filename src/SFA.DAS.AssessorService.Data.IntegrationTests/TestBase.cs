using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class TestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            new DatabaseService().SetupDatabase();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            new DatabaseService().DropDatabase();
        }
    }
}
