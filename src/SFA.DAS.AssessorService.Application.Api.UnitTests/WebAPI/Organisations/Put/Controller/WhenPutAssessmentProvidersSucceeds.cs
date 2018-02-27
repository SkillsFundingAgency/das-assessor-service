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
        private static UpdateOrganisationRequest _organisationUpdateViewModel;
        private static Organisation _organisationQueryViewModel;

        Establish context = () =>
        {
            Setup();

            _organisationQueryViewModel = Builder<Organisation>.CreateNew().Build();

            Mediator.Setup(q => q.Send(Moq.It.IsAny<UpdateOrganisationRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((_organisationQueryViewModel)));

            _organisationUpdateViewModel = Builder<UpdateOrganisationRequest>.CreateNew()                   
                    .Build();

            OrganisationContoller = new OrganisationController(
                  Mediator.Object,
                  StringLocalizer.Object,                 
                  Logger.Object);
        };

        Because of = () =>
        {
            Result = OrganisationContoller.UpdateOrganisation(_organisationUpdateViewModel).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = Result as Microsoft.AspNetCore.Mvc.NoContentResult;
            result.Should().NotBeNull();
        };
    }
}
