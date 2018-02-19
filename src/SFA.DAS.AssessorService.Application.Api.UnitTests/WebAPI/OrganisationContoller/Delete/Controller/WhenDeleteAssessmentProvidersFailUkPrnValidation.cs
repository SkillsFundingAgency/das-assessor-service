namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller.Put
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Controllers;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Threading.Tasks;

    [Subject("AssessorService")]
    public class WhenDeleteAssessmentProvidersFailUkPrnValidation : WhenPutAssessmentProvidersTestBase
    {
        private static OrganisationDeleteViewModel _organisationUpdateViewModel;

        Establish context = () =>
        {
            Setup();

            _organisationUpdateViewModel = Builder<OrganisationDeleteViewModel>.CreateNew().Build();          

            OrganisationContoller = new OrganisationController(
                  Mediator.Object,
                  StringLocalizer.Object,
                  UkPrnValidator,
                  Logger.Object);
        };

        Because of = () =>
        {
            Result = OrganisationContoller.Delete(999).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = Result as Microsoft.AspNetCore.Mvc.BadRequestObjectResult;
            result.Should().NotBeNull();
        };
    }
}
