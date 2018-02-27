//namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller
//{
//    using FluentAssertions;
//    using Machine.Specifications;
//    using Microsoft.AspNetCore.Mvc;
//    using SFA.DAS.AssessorService.Api.Types;
//    using SFA.DAS.AssessorService.Application.Api.Controllers;
//    using System.Threading.Tasks;

//    [Subject("AssessorService")]
//    public class WhenGetAssessmentProvidersGetUkPrnValidationFails : WhenGetAssessmentProvidersTestBase
//    {
//        Establish context = () =>
//        {
//            Setup();

//            GetOrganisationsOrchestratorMock.Setup(q => q.GetOrganisation(Moq.It.IsAny<int>()))
//                .Returns(Task.FromResult((_organisationQueryViewModel)));

//            OrganisationContoller = new OrganisationQueryController(
              
//               OrganisationQueryRepositoryMock.Object,
//               StringLocalizer.Object,
//               UkPrnValidator,
//               Logger.Object);
//        };

//        Because of = () =>
//        {
//            Result = OrganisationContoller.Get(10).Result;
//        };

//        private Machine.Specifications.It verify_succesfully = () =>
//        {
//            Result.Should().BeOfType<BadRequestObjectResult>();
//        };
//    }
//}
