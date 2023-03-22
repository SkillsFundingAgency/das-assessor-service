using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Orchestrators.OrganisationSearch
{
    public class WhenOrganisationSearchByUkprnIsCalled : OrganisationSearchTestBase
    {
        [TestCase(12345678, "Blue Barns Limited", 12345678, true)]
        [TestCase(41235678, "Green Grass Limited", 41235678, true)]
        [TestCase(61234578, "Yellow Sun Limited", 61234578, false)]
        public async Task ThenUkprnFoundCorrectlyForApprovedOrganisations(int searchTerm, string expectedCompanyName, int expectedUkprn, bool expectedRoEPAOApproved)
        {
            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _epaOrganisationValidator.Object);
            var results = await sut.OrganisationSearchByUkprn(searchTerm);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(results.Single().Ukprn, Is.EqualTo(expectedUkprn));
                Assert.That(results.Single().Name, Is.EqualTo(expectedCompanyName));
                Assert.That(results.Single().RoEPAOApproved, Is.EqualTo(expectedRoEPAOApproved));
            });
        }
    }
}
