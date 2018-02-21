using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Application.MSpec.UnitTests
{
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
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

        Machine.Specifications.It should_call_the_api = () =>
        {

            //TokenService.Verify(serv => serv.GetJwt(), Times.AtMostOnce);
        };

        private Machine.Specifications.It should_get_an_organisation = () =>
        {
            //OrganisationService.Verify(serv => serv.GetOrganisation("jwt", 12345));
            ApiClient.Verify(a => a.Get("12345", "12345"));
        };

        Machine.Specifications.It should_return_a_viewresult = () =>
        {
            _actionResult.Should().BeOfType<ViewResult>();
        };

        Machine.Specifications.It should_return_an_organisation = () =>
        {
            var result = _actionResult as ViewResult;
            result.Model.Should().BeOfType<OrganisationQueryViewModel>();
        };
    }
}
