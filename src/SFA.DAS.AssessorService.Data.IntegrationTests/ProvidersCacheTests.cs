using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    [TestFixture]
    public class ProvidersCacheTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        private SqlConnection _databaseConnection;
        private UnitOfWork _unitOfWork;
        private IApprovalsExtractRepository _repository;
        private Mock<IRoatpApiClient> _roatpApiClientMock;

        [OneTimeSetUp]
        public void CommonSetup()
        {
            _databaseConnection = new SqlConnection(_databaseService.SqlConnectionStringTest);
            _unitOfWork = new UnitOfWork(_databaseConnection);
            _roatpApiClientMock = new Mock<IRoatpApiClient>();
            _repository = new ApprovalsExtractRepository(_unitOfWork, _roatpApiClientMock.Object, new Mock<ILogger<ApprovalsExtractRepository>>().Object);
        }

        [Test]
        public async Task When_ProviderNotExist_Then_ProviderInserted()
        {
            // Arrange

            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract;");
            _databaseService.Execute("INSERT INTO ApprovalsExtract (ApprenticeshipId, FirstName, LastName, UKPRN) VALUES (123, 'TestFirstName', 'TestLastName', '10000528');");
            _roatpApiClientMock.Setup(m => m.SearchOrganisationByUkprn(It.IsAny<int>()))
                .ReturnsAsync(new OrganisationSearchResult[] { new OrganisationSearchResult() { ProviderName = "Test Provider Name" }   });
        
            // Act

            await _repository.InsertProvidersFromApprovalsExtract();

            // Assert

            IEnumerable<dynamic> providers;
            using (var connection = new SqlConnection(_databaseConnection.ConnectionString))
            {
                providers = await connection.QueryAsync("SELECT * FROM Providers;");
            }
            providers.Should().NotBeNull();
            providers.Should().HaveCount(1);
            long ukprn = providers.ElementAt(0).Ukprn;
            ukprn.Should().Be(10000528);
            string name = providers.ElementAt(0).Name;
            name.Should().Be("Test Provider Name");
        }
    }
}
