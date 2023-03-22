using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Orchestrators.OrganisationSearch
{
    public class WhenOrganisationSearchByEpao : OrganisationSearchTestBase
    {
        [TestCase("EPA0006")]
        public async Task ThenEmptyCollectionReturnedIfMatchFails(string searchTerm)
        {
            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _epaOrganisationValidator.Object);
            var results = await sut.OrganisationSearchByEpao(searchTerm);

            // Assert
            Assert.That(results, Is.Empty);
        }

        [TestCase("EPA0001", "Blue Barns Limited", "EPA0001", true)]
        [TestCase("EPA0004", "Green Grass Limited", "EPA0004", true)]
        public async Task ThenResultHasMatchingIdAndNameAndReEPAOApproved(string searchTerm, string expectedCompanyName, string expectedId, bool expectedRoEPAOApproved)
        {
            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _epaOrganisationValidator.Object);
            var results = await sut.OrganisationSearchByEpao(searchTerm);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(results.Single().Name, Is.EqualTo(expectedCompanyName));
                Assert.That(results.Single().Id, Is.EqualTo(expectedId));
                Assert.That(results.Single().RoEPAOApproved, Is.EqualTo(expectedRoEPAOApproved));
            });
        }
    }
}
