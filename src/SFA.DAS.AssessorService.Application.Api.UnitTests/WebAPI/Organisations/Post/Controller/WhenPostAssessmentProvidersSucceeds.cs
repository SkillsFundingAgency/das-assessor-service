namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Controllers;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Threading;
    using System.Threading.Tasks;

    [Subject("AssessorService")]
    public class WhenPostAssessmentProvidersSucceeds : WhenPostAssessmentProvidersTestBase
    {
        private static OrganisationCreateViewModel _organisationCreateViewModel;
        private static OrganisationQueryViewModel _organisationQueryViewModel;

        Establish context = () =>
        {
            Setup();

            _organisationQueryViewModel = Builder<OrganisationQueryViewModel>.CreateNew().Build();

            Mediator.Setup(q => q.Send(Moq.It.IsAny<OrganisationCreateViewModel>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((_organisationQueryViewModel)));

            _organisationCreateViewModel = Builder<OrganisationCreateViewModel>.CreateNew()
                    .With(x => x.EndPointAssessorUKPRN = 10000000)
                    .Build();

            OrganisationContoller = new OrganisationController(
                  Mediator.Object,
                  StringLocalizer.Object,                 
                  Logger.Object);
        };

        Because of = () =>
        {
            Result = OrganisationContoller.CreateOrganisation(_organisationCreateViewModel).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = Result as Microsoft.AspNetCore.Mvc.CreatedAtRouteResult;
            result.Should().NotBeNull();
        };
    }
}
