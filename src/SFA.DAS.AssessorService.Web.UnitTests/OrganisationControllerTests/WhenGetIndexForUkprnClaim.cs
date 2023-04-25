using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetIndexForUkprnClaim : OrganisationControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            base.Arrange(addEpaoClaim: false, addUkprnClaim: true);
        }

        [Test]
        public async Task Should_get_an_organisation_by_ukprn()
        {
            _actionResult = await sut.Index();
            OrganisationApiClient.Verify(a => a.Get("12345"));
        }

        [Test]
        public async Task Should_return_a_viewresult()
        {
            _actionResult = await sut.Index();
            _actionResult.Should().BeOfType<ViewResult>();
        }

        [Test]
        public async Task Should_return_an_organisation()
        {
            _actionResult = await sut.Index();
            var result = _actionResult as ViewResult;
            result.Model.Should().BeOfType<OrganisationResponse>();
        }
    }
}
