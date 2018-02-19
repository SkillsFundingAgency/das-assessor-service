namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller.Put
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Controllers;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Threading.Tasks;

    [Subject("AssessorService")]
    public class WhenPostAssessmentProvidersFailUkPrnValidation : WhenPutAssessmentProvidersTestBase
    {
        private static OrganisationUpdateViewModel _organisationUpdateViewModel;

        Establish context = () =>
        {
            Setup();

            _organisationUpdateViewModel = Builder<OrganisationUpdateViewModel>.CreateNew().Build();          

            OrganisationContoller = new OrganisationController(
                  Mediator.Object,
                  OrganizationRepository.Object,
                  StringLocalizer.Object,
                  UkPrnValidator,
                  Logger.Object);
        };

        Because of = () =>
        {
            Result = OrganisationContoller.Update(999, _organisationUpdateViewModel).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = Result as Microsoft.AspNetCore.Mvc.BadRequestObjectResult;
            result.Should().NotBeNull();
        };
    }
}
