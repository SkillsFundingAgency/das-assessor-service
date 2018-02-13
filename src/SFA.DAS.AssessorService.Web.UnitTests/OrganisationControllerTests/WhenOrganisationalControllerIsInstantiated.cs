namespace SFA.DAS.AssessorService.Application.MSpec.UnitTests
{
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.AspNetCore.Authorization;
    using SFA.DAS.AssessorService.Web.Controllers;
    using SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests;

    [Subject("OrganisationController")]
    public class WhenOrganisationalControllerIsInstantiated : OrganisationControllerTestBase
    {
        Establish context = () =>
        {
            Setup();
        };

        Because of = () =>
        {

        };

        Machine.Specifications.It should_have_authorize_attribute = () =>
        {
            typeof(OrganisationController).Should().BeDecoratedWith<AuthorizeAttribute>();
        };
    }
}
