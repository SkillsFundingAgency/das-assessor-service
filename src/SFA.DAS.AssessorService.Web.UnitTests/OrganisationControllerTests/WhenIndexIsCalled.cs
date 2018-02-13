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

        Machine.Specifications.It should_get_s_token = () =>
        {
            TokenService.Verify(serv => serv.GetJwt(), Times.AtMostOnce);
        };

        Machine.Specifications.It should_get_an_oorganisation = () =>
        {
            OrganisationService.Verify(serv => serv.GetOrganisation("jwt"));
        };

        Machine.Specifications.It should_return_a_viewresult = () =>
        {
            _actionResult.Should().BeOfType<ViewResult>();
        };

        Machine.Specifications.It should_return_a_prganisation = () =>
        {
            var result = _actionResult as ViewResult;
            result.Model.Should().BeOfType<Organisation>();
        };
    }
}
