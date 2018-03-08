using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    using Api.Types.Models;

    [TestFixture]
    public class WhenIndexIsCalled : OrganisationControllerTestBase
    {
        private static IActionResult _actionResult;

        [SetUp]
        public void Arrange()
        {
            Setup();
        }

        [Test]
        public void Should_Call_The_Api()
        {

            //TokenService.Verify(serv => serv.GetJwt(), Times.AtMostOnce);
        }

        [Test]
        public void Should_get_an_organisation()
        {
            _actionResult = OrganisationController.Index().Result;
            //OrganisationService.Verify(serv => serv.GetOrganisation("jwt", 12345));
            ApiClient.Verify(a => a.Get("12345"));
        }

        [Test]
        public void Should_return_a_viewresult()
        {
            _actionResult = OrganisationController.Index().Result;
            _actionResult.Should().BeOfType<ViewResult>();
        }

        [Test]
        public void Should_return_an_organisation()
        {
            _actionResult = OrganisationController.Index().Result;
            var result = _actionResult as ViewResult;
            result.Model.Should().BeOfType<OrganisationResponse>();
        }
    }
}
