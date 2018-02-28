//namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller
//{
//    using FizzWare.NBuilder;
//    using FluentAssertions;
//    using Machine.Specifications;
//    using SFA.DAS.AssessorService.Api.Types;
//    using SFA.DAS.AssessorService.Application.Api.Controllers;
//    using System.Threading;
//    using System.Threading.Tasks;
//    using AssessorService.Api.Types.Models;

//    [Subject("AssessorService")]
//    public class WhenPostAssessmentProvidersSucceeds : WhenPostAssessmentProvidersTestBase
//    {
//        private static CreateOrganisationRequest _organisationCreateViewModel;
//        private static Organisation _organisationQueryViewModel;

//        Establish context = () =>
//        {
//            Setup();

//            _organisationQueryViewModel = Builder<Organisation>.CreateNew().Build();

//            Mediator.Setup(q => q.Send(Moq.It.IsAny<CreateOrganisationRequest>(), Moq.It.IsAny<CancellationToken>()))
//                .Returns(Task.FromResult((_organisationQueryViewModel)));

//            _organisationCreateViewModel = Builder<CreateOrganisationRequest>.CreateNew()
//                    .With(x => x.EndPointAssessorUKPRN = 10000000)
//                    .Build();

//            OrganisationContoller = new OrganisationController(
//                  Mediator.Object,
//                  StringLocalizer.Object,                 
//                  Logger.Object);
//        };

//        Because of = () =>
//        {
//            Result = OrganisationContoller.CreateOrganisation(_organisationCreateViewModel).Result;
//        };

//        Machine.Specifications.It verify_succesfully = () =>
//        {
//            var result = Result as Microsoft.AspNetCore.Mvc.CreatedAtRouteResult;
//            result.Should().NotBeNull();
//        };
//    }
//}
