using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Exceptions;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Query
{
    public class WhenSearchOrganisationCannotFindUkPrn : OrganisationQueryBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
        }

        [Test]
        public void ThenTheResultReturnsNotFoundStatus()
        {
            Assert.ThrowsAsync<ResourceNotFoundException>(async () => await OrganisationQueryController.SearchOrganisation(10000000));
        }
    }
}
