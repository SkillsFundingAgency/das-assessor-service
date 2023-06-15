using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Orchestrators.OrganisationSearch
{
    public class WhenOrganisationSearchByNameOrCharityNumberOrCompanyNumberIsCalled_WithRename : OrganisationSearchTestBase
    {
        [TestCase("Green Grass Limited (New Name)", "Green Grass Limited", "00030004")]
        [TestCase("White Moon Limited (New Name)", "White Moon Limited", "00030008")]
        [TestCase("Purple Flower Ltd (New Name)", "Purple Flower Ltd", "00030009")]
        [Ignore("Tested code to be removed")]
        public async Task ThenExistingOrganisationFoundByCompanyNumberWhereCompanyNameHasChanged(string newCompanyName, string oldCompanyName, string companyNumber)
        {
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object);
            var results = await sut.OrganisationSearchByNameOrCharityNumberOrCompanyNumber(newCompanyName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(results.Single().CompanyNumber, Is.EqualTo(companyNumber));
                Assert.That(results.Single().Name, Is.EqualTo(oldCompanyName));
                Assert.That(results.Single().RoEPAOApproved, Is.True);
            });
        }

        [TestCase("Large Giving Limited (New Name)", "Large Giving Limited", "00040010")]
        [TestCase("Medium Giving Limited (New Name)", "Medium Giving Limited", "00040011")]
        [TestCase("Small Giving Ltd (New Name)", "Small Giving Ltd", "00040012")]
        [Ignore("Tested code to be removed")]
        public async Task ThenExistingOrganisationFoundByCharityNumberWhereCharityNameHasChanged(string newCharityName, string oldCharityName, string charityNumber)
        {
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object);
            var results = await sut.OrganisationSearchByNameOrCharityNumberOrCompanyNumber(newCharityName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(results.Single().CharityNumber, Is.EqualTo(charityNumber));
                Assert.That(results.Single().Name, Is.EqualTo(oldCharityName));
                Assert.That(results.Single().RoEPAOApproved, Is.True);
            });
        }
    }
}
