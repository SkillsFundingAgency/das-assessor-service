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
    public class StandardsGetTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        private SqlConnection _databaseConnection;
        private UnitOfWork _unitOfWork;
        private StandardRepository _repository;

        private List<StandardModel> _standards;

        [OneTimeSetUp]
        public void SetupStandardsTable()
        {
            _databaseConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
            _unitOfWork = new UnitOfWork(_databaseConnection);
            _repository = new StandardRepository(_unitOfWork);
            
            _standards = GetListOfStandardVersions();

            StandardsHandler.InsertRecords(_standards);
        }

        //[TestCase("ST0001", "1", "ST0001_1.0")]
        //[TestCase("ST0001", "1.0", "ST0001_1.0")]
        //[TestCase("ST0001", "1.00", "ST0001_1.0")]
        //[TestCase("ST0001", "1.1", "ST0001_1.1")]
        //[TestCase("ST0001", "1.2", "ST0001_1.2")]
        //[TestCase("ST0001", "", "ST0001_1.2")]
        [TestCase("ST0001", "1.10", "ST0001_1.10")]
        [TestCase("ST0001", "1.12", "ST0001_1.12")]
        public async Task GetStandardByStandardReferenceAndVersion_ReturnsCorrectStandard(string standardReference, string version, string standardUId)
        {
            var expectedStandard = _standards.Single(s => s.StandardUId == standardUId);

            var standard = await _repository.GetStandardVersionByIFateReferenceNumber(standardReference, version);

            Assert.AreEqual(expectedStandard.StandardUId, standard.StandardUId);
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

        private List<StandardModel> GetListOfStandardVersions()
        {
            return new List<StandardModel>{
                new StandardModel
                {
                    StandardUId = "ST0001_1.0",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.0",
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    EPAChanged = false
                },
                new StandardModel
                {
                    StandardUId = "ST0001_1.1",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.1",
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    EPAChanged = false
                },
                 new StandardModel
                {
                    StandardUId = "ST0001_1.2",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.2",
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    EPAChanged = false
                },
                new StandardModel
                {
                    StandardUId = "ST0001_1.10",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.10",
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    EPAChanged = false
                },
                new StandardModel
                {
                    StandardUId = "ST0001_1.12",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.12",
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    EPAChanged = false
                }
            };
        }
    }
}
