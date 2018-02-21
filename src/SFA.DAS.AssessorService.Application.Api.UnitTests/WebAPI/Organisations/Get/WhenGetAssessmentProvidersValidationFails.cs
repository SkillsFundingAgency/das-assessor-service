namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller
{
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.AspNetCore.Mvc;
    using SFA.DAS.AssessorService.Application.Api.Controllers;

    [Subject("AssessorService")]
    public class WhenGetAssessmentProvidersGetUkPrnValidationFails : WhenGetAssessmentProvidersTestBase
    {
        Establish context = () =>
        {
            Setup();

            OrganisationContoller = new OrganisationQueryController(
               OrganisationQueryRepositoryMock.Object,
               StringLocalizer.Object,
               UkPrnValidator,
               Logger.Object);
        };

        Because of = () =>
        {
            Result = OrganisationContoller.Get(10).Result;
        };

        private Machine.Specifications.It verify_succesfully = () =>
        {
            Result.Should().BeOfType<BadRequestObjectResult>();
        };
    }
}
