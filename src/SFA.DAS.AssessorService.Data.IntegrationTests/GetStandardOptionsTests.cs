using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    [TestFixture]
    public class GetStandardOptionsTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        private SqlConnection _databaseConnection;
        private UnitOfWork _unitOfWork;
        private StandardRepository _repository;

        private List<StandardOptionModel> _options;
        private List<StandardModel> _standards;

        private StandardOptions _expectedStandardResult;

        [OneTimeSetUp]
        public void SetupStandardOptionsTable()
        {
            _databaseConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
            _unitOfWork = new UnitOfWork(_databaseConnection);
            _repository = new StandardRepository(_unitOfWork);

            _options = GetListOfOptions();
            _standards = GetListOfStandardVersions();

            StandardOptionsHandler.InsertRecords(_options);
            StandardsHandler.InsertRecords(_standards);

            _expectedStandardResult = new StandardOptions
            {
                StandardUId = "ST0001_1.1",
                StandardReference = "ST0001",
                StandardCode = 1,
                Version = "1.1",
                CourseOption = new List<string>
                {
                    "ST0001_1.1 Option 1",
                    "ST0001_1.1 Option 2"
                }
            };
        }

        [Test]
        public async Task GetAllStandardOptions_ReturnsFullListOfStandardOptions()
        {
            var results = await _repository.GetAllStandardOptions();

            results.Count().Should().Be(_standards.Count);
            results.First(result => result.StandardUId == "ST0001_1.1").Should().BeEquivalentTo(_expectedStandardResult);
        }

        [Test]
        public async Task GetLatestVersionStandardOptions_ReturnsListOfStandardOptions()
        {
            var results = await _repository.GetStandardOptionsForLatestStandardVersions();

            results.Count().Should().Be(2);
            results.First(result => result.StandardUId == "ST0001_1.1").Should().BeEquivalentTo(_expectedStandardResult);
        }

        [OneTimeTearDown]
        public void TearDownTables()
        {
            StandardOptionsHandler.DeleteAllRecords();
            StandardsHandler.DeleteAllRecords();

            if (_databaseConnection != null)
            {
                _databaseConnection.Dispose();
            }
        }

        private List<StandardOptionModel> GetListOfOptions()
        {
            return new List<StandardOptionModel>
            {
                new StandardOptionModel { StandardUId = "ST0001_1.0", OptionName = "ST0001_1.0 Option 1" },
                new StandardOptionModel { StandardUId = "ST0001_1.0", OptionName = "ST0001_1.0 Option 2" },
                new StandardOptionModel { StandardUId = "ST0001_1.1", OptionName = "ST0001_1.1 Option 1" },
                new StandardOptionModel { StandardUId = "ST0001_1.1", OptionName = "ST0001_1.1 Option 2" },
                new StandardOptionModel { StandardUId = "ST0002_1.0", OptionName = "ST0002_1.0 Option 1" },
                new StandardOptionModel { StandardUId = "ST0002_1.0", OptionName = "ST0002_1.0 Option 2" }
            };
        }

        private List<StandardModel> GetListOfStandardVersions()
        {
            return new List<StandardModel>{
                new StandardModel
                {
                    StandardUId = "ST0001_1.0",
                    IFateReferenceNumber = "ST0001",
                    LarsCode = 1,
                    Version = "1.0",
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
                },
                new StandardModel
                {
                    StandardUId = "ST0001_1.1",
                    IFateReferenceNumber = "ST0001",
                    LarsCode = 1,
                    Version = "1.1",
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
                },
                 new StandardModel
                {
                    StandardUId = "ST0002_1.0",
                    IFateReferenceNumber = "ST0002",
                    LarsCode = 2,
                    Version = "1.0",
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
                },
                new StandardModel
                {
                    StandardUId = "ST0003_1.0",
                    IFateReferenceNumber = "ST0003",
                    LarsCode = 3,
                    Version = "1.0",
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
                }
            };
        }
    }
}
