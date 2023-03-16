using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.OrganisationSearch.OrganisationSearchPaged
{
    public class WhenCalledAndIsValidEpaOrganisationId
    {
        private Mock<ILogger<OrganisationSearchController>> _logger;
        private Mock<IOrganisationSearchOrchestrator> _organisationSearchOrchestrator;

        private Mock<ICompaniesHouseApiClient> _companiesHouseApiClient;
        private Mock<ICharityCommissionApiClient> _charityCommissionApiClient;

        private IEnumerable<OrganisationSearchResult> _searchResults;

        [Test]
        public async Task ThenOrganisationSearchByEpaoIsCalled()
        {
            // Arrange
            _logger = new Mock<ILogger<OrganisationSearchController>>();

            _organisationSearchOrchestrator = new Mock<IOrganisationSearchOrchestrator>();
            _organisationSearchOrchestrator.Setup(p => p.IsValidEpaOrganisationId(It.IsAny<string>())).Returns(true);

            _searchResults = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    LegalName = "Company One Limited"
                }
            };

            _organisationSearchOrchestrator.Setup(p => p.OrganisationSearchByEpao(It.IsAny<string>())).ReturnsAsync(_searchResults);

            _companiesHouseApiClient = new Mock<ICompaniesHouseApiClient>();
            _charityCommissionApiClient = new Mock<ICharityCommissionApiClient>();

            // Act
            var sut = new OrganisationSearchController(_logger.Object, _organisationSearchOrchestrator.Object, _companiesHouseApiClient.Object, _charityCommissionApiClient.Object);
            var result = await sut.OrganisationSearchPaged("EPA0001", 10, 1);

            // Assert
            _organisationSearchOrchestrator.Verify(p => p.IsValidEpaOrganisationId("EPA0001"), Times.Once);
            _organisationSearchOrchestrator.Verify(p => p.OrganisationSearchByEpao("EPA0001"), Times.Once);
        }
    }
}
