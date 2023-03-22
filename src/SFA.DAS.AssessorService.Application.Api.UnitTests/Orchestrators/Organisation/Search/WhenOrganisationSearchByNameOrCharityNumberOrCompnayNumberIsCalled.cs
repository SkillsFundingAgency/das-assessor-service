using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Orchestrators.OrganisationSearch
{
    public class WhenOrganisationSearchByNameOrCharityNumberOrCompanyNumberIsCalled_Example : OrganisationSearchTestBase
    {
        [TestCase("Blue")]
        [TestCase("Green")]
        [TestCase("Yellow")]
        [TestCase("Green Grass Limited")]
        public async Task ThenOrganisationReturnedIfNameContainsSearchTerm(string searchTerm)
        {
            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _epaOrganisationValidator.Object);
            var results = await sut.OrganisationSearchByNameOrCharityNumberOrCompanyNumber(searchTerm);

            // Assert
            Assert.That(results.All(r => r.Name.Contains(searchTerm)));
        }

        [TestCase("Green Aliens Conglomerate")] // matches "Green" in "Green Grass Limited"
        [TestCase("Blah Blah Limited")] // matches "Limited" in "Blue Barns Limited" or "Green Grass Limited"
        public async Task ThenOrganisationReturnedIfAnyWordInSearchTermMatches(string searchTerm)
        {
            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _epaOrganisationValidator.Object);
            var results = await sut.OrganisationSearchByNameOrCharityNumberOrCompanyNumber(searchTerm);

            // Assert
            Assert.That(results.All(r => r.Name.Split(' ')
                                               .Intersect(searchTerm.Split(' '))
                                               .Any()));
        }

        [TestCase("Lollipop")]
        [TestCase("Space Pirates Inc.")]
        [TestCase("Rawrrr rawrrrr")]
        [TestCase("98765432")]
        public async Task ThenEmptyCollectionReturnedIfMatchFails(string searchTerm)
        {
            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _epaOrganisationValidator.Object);
            var results = await sut.OrganisationSearchByNameOrCharityNumberOrCompanyNumber(searchTerm);

            // Assert
            Assert.That(results, Is.Empty);
        }

        [TestCase("00030004", "00030004", "Green Grass Limited")]
        [TestCase("00030001", "00030001", "Blue Barns Limited")]
        [TestCase("00030002", "00030002", "Sky Blue Ltd")]
        public async Task ThenResultHasMatchingCompanyNumberAndName(string searchTerm, string expectedCompanyNumber, string expectedCompanyName)
        {
            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _epaOrganisationValidator.Object);
            var results = await sut.OrganisationSearchByNameOrCharityNumberOrCompanyNumber(searchTerm);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(results.Single().CompanyNumber, Is.EqualTo(expectedCompanyNumber));
                Assert.That(results.Single().Name, Is.EqualTo(expectedCompanyName));
            });
        }
    }
}
