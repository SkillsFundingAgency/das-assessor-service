namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller.Put
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Controllers;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Threading;
    using System.Threading.Tasks;

    [Subject("AssessorService")]
    public class WhenPutAssessmentProvidersSucceeds : WhenPutAssessmentProvidersTestBase
    {
        private static OrganisationUpdateViewModel _organisationUpdateViewModel;
        private static OrganisationQueryViewModel _organisationQueryViewModel;

        Establish context = () =>
        {
            Setup();

            _organisationQueryViewModel = Builder<OrganisationQueryViewModel>.CreateNew().Build();

            Mediator.Setup(q => q.Send(Moq.It.IsAny<OrganisationUpdateViewModel>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((_organisationQueryViewModel)));

            _organisationUpdateViewModel = Builder<OrganisationUpdateViewModel>.CreateNew()                   
                    .Build();

            OrganisationContoller = new OrganisationController(
                  Mediator.Object,
                  OrganizationRepository.Object,
                  StringLocalizer.Object,
                  UkPrnValidator,
                  Logger.Object);
        };

        Because of = () =>
        {
            Result = OrganisationContoller.Update(10000000, _organisationUpdateViewModel).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = Result as Microsoft.AspNetCore.Mvc.NoContentResult;
            result.Should().NotBeNull();
        };
    }
}
