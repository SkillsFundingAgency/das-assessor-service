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
    public class WhenCalled
    {
        private Mock<ILogger<OrganisationSearchController>> _logger;
        private Mock<IOrganisationSearchOrchestrator> _organisationSearchOrchestrator;

        private Mock<ICompaniesHouseApiClient> _companiesHouseApiClient;
        private Mock<ICharityCommissionApiClient> _charityCommissionApiClient;

        private IEnumerable<OrganisationSearchResult> _searchResults;

        [Test]
        public async Task ThenOrganisationSearchByUkprnIsCalled()
        {
            // Arrange
            _logger = new Mock<ILogger<OrganisationSearchController>>();

            _organisationSearchOrchestrator = new Mock<IOrganisationSearchOrchestrator>();
            _organisationSearchOrchestrator.Setup(p => p.IsValidEpaOrganisationId(It.IsAny<string>())).Returns(false);

            int ukprn;
            _organisationSearchOrchestrator.Setup(p => p.IsValidUkprn(It.IsAny<string>(), out ukprn))
                .Callback(new ValidateUkprn((string searchTerm, out int ukprn) => { ukprn = int.Parse(searchTerm); }))
                .Returns(true);

            _searchResults = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    LegalName = "Company One Limited",
                    Ukprn = 12345678
                }
            };

            _organisationSearchOrchestrator.Setup(p => p.OrganisationSearchByUkprn(It.IsAny<int>())).ReturnsAsync(_searchResults);
            _organisationSearchOrchestrator.Setup(p => p.Dedupe(It.IsAny<IEnumerable<OrganisationSearchResult>>())).Returns(_searchResults);

            _companiesHouseApiClient = new Mock<ICompaniesHouseApiClient>();
            _charityCommissionApiClient = new Mock<ICharityCommissionApiClient>();

            // Act
            var sut = new OrganisationSearchController(_logger.Object, _organisationSearchOrchestrator.Object, _companiesHouseApiClient.Object, _charityCommissionApiClient.Object);
            var result = await sut.OrganisationSearchPaged("12345678", 10, 1);

            // Assert
            _organisationSearchOrchestrator.Verify(p => p.IsValidEpaOrganisationId("12345678"), Times.Once);
            _organisationSearchOrchestrator.Verify(p => p.IsValidUkprn("12345678", out ukprn), Times.Once);
            _organisationSearchOrchestrator.Verify(p => p.OrganisationSearchByUkprn(12345678), Times.Once);
        }

        private delegate void ValidateUkprn(string searchTerm, out int ukprn);
    }
}
