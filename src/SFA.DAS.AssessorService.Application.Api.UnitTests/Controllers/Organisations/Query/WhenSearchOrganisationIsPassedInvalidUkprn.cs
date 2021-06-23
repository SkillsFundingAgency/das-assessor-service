using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Exceptions;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Query
{
    public class WhenSearchOrganisationIsPassedInvalidUkprn : OrganisationQueryBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
        }

        [Test]
        public void ThenTheResultReturnBadRequestStatus()
        {
            Assert.ThrowsAsync<BadRequestException>(async () => await OrganisationQueryController.SearchOrganisation(10));
        }
    }
}
