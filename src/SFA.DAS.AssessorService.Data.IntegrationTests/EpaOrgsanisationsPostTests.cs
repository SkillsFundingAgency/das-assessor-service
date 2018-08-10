using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class EpaOrgsanisationsPostTests: TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private  RegisterRepository _repository;

        [OneTimeSetUp]
        public void SetUpOrganisationTests()
        {
           _repository = new RegisterRepository(_databaseService.WebConfiguration);
        }
    }
}
