namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Controllers;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Threading.Tasks;

    [Subject("AssessorService")]
    public class WhenPostAssessmentProvidersFailUkPrnValidation : WhenPostAssessmentProvidersTestBase
    {
        private static OrganisationCreateViewModel _organisationCreateViewModel;

        Establish context = () =>
        {
            Setup();

            _organisationCreateViewModel = Builder<OrganisationCreateViewModel>.CreateNew().Build();          

            OrganisationContoller = new OrganisationController(
                  Mediator.Object,
                  OrganizationRepository.Object,
                  StringLocalizer.Object,
                  UkPrnValidator,
                  Logger.Object);
        };

        Because of = () =>
        {
            Result = OrganisationContoller.Create(999, _organisationCreateViewModel).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = Result as Microsoft.AspNetCore.Mvc.BadRequestObjectResult;
            result.Should().NotBeNull();
        };
    }
}
