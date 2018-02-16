namespace SFA.DAS.AssessorService.Application.MSpec.UnitTests
{
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using SFA.DAS.AssessorService.Web.Controllers;
    using SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests;
    using SFA.DAS.AssessorService.Web.ViewModels;

    [Subject("OrganisationController")]
    public class WhenIndexIsCalled : OrganisationControllerTestBase
    {
        private static IActionResult _actionResult;

        Establish context = () =>
    {
        Setup();
    };

        Because of = () =>
        {
            _actionResult = OrganisationController.Index().Result;
        };

        Machine.Specifications.It should_get_a_token = () =>
        {
            TokenService.Verify(serv => serv.GetJwt(), Times.AtMostOnce);
        };

        Machine.Specifications.It should_get_an_organisation = () =>
        {
            OrganisationService.Verify(serv => serv.GetOrganisation("jwt", 12345));
        };

        Machine.Specifications.It should_return_a_viewresult = () =>
        {
            _actionResult.Should().BeOfType<ViewResult>();
        };

        Machine.Specifications.It should_return_an_organisation = () =>
        {
            var result = _actionResult as ViewResult;
            result.Model.Should().BeOfType<Organisation>();
        };
    }
}
