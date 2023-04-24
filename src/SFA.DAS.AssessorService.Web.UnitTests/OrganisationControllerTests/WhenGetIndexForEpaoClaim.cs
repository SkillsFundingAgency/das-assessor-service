using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    using Api.Types.Models;
    using System.Threading.Tasks;

    [TestFixture]
    public class WhenGetIndexForEpaoClaim : OrganisationControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            base.Arrange(addEpaoClaim: true, addUkprnClaim: false);
        }

        [Test]
        public async Task Should_get_an_organisation_by_epao()
        {
            _actionResult = await sut.Index();
            OrganisationApiClient.Verify(a => a.GetEpaOrganisation(EpaoId));
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
