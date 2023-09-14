using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class TestBase
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            var databaseService = new DatabaseService();
            await databaseService.SetupDatabase();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            new DatabaseService().DropDatabase();
        }
    }
}
