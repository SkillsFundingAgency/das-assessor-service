namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Controllers;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Threading.Tasks;

    [Subject("AssessorService")]
    public class WhenGetAssessmentProvidersGetCannotFindUkPrn : WhenGetAssessmentProvidersTestBase
    {
        private static Organisation _organisationQueryViewModel;

        Establish context = () =>
        {
            Setup();

            _organisationQueryViewModel = Builder<Organisation>.CreateNew().Build();
            OrganisationQueryRepositoryMock.Setup(q => q.GetByUkPrn(Moq.It.IsAny<int>()))
                .Returns(Task.FromResult<Organisation>(null));

            OrganisationContoller = new OrganisationQueryController(
                  OrganisationQueryRepositoryMock.Object,
                  StringLocalizer.Object,
                  UkPrnValidator,
                  Logger.Object);
        };

        Because of = () =>
        {
            Result = OrganisationContoller.Get(10000000).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = Result as Microsoft.AspNetCore.Mvc.NotFoundObjectResult;
            result.Should().NotBeNull();
        };
    }
}
