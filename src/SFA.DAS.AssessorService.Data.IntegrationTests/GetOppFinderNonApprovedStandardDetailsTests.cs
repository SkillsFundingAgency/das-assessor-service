using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    [TestFixture]
    public class GetOppFinderNonApprovedStandardDetailsTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        private SqlConnection _databaseConnection;
        private UnitOfWork _unitOfWork;
        private OppFinderRepository _repository;

        private List<StandardModel> _nonApprovedStandards;

        [OneTimeSetUp]
        public void SetUpStandardsTable()
        {
            _databaseConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
            _unitOfWork = new UnitOfWork(_databaseConnection);
            _repository = new OppFinderRepository(_unitOfWork);

            _nonApprovedStandards = GetListOfNonApprovedStandard();

            StandardsHandler.InsertRecords(_nonApprovedStandards);
        }

        [TestCase("ST0001")]
        public async Task GetOppFinderNonApprovedStandardDetails_ReturnsCorrectNonApprovedStandard(string standardReference)
        {
            var expectedStandard = _nonApprovedStandards.Single(s => s.IFateReferenceNumber == standardReference);

            var standard = await _repository.GetOppFinderNonApprovedStandardDetails(standardReference);

            standard.Title.Should().Be(expectedStandard.Title);
            standard.Status.Should().Be(expectedStandard.Status);
            standard.IFateReferenceNumber.Should().Be(expectedStandard.IFateReferenceNumber);
            standard.Level.Should().Be(expectedStandard.Level.ToString());
            standard.OverviewOfRole.Should().Be(expectedStandard.OverviewOfRole);
            standard.Route.Should().Be(expectedStandard.Route);
            standard.TypicalDuration.Should().Be(expectedStandard.TypicalDuration);
            standard.TrailblazerContact.Should().Be(expectedStandard.TrailblazerContact);
            standard.StandardPageUrl.Should().Be(expectedStandard.StandardPageUrl);
        }

        [OneTimeTearDown]
        public void TearDownStandardsTable()
        {
            StandardsHandler.DeleteAllRecords();

            if (_databaseConnection != null)
            {
                _databaseConnection.Dispose();
            }
        }

        private List<StandardModel> GetListOfNonApprovedStandard()
        {
            return new List<StandardModel>
            {
                new StandardModel
                {
                    StandardUId = "ST0001_1.0",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.0",
                    Title = "Standard",
                    Level = 4,
                    Status = "In development",
                    TypicalDuration = 12,
                    TrailblazerContact = "Contact name",
                    StandardPageUrl = "www.standard.com",
                    OverviewOfRole = "Explanation of apprenticeship job role"
                }
            };
        }
    }
}
