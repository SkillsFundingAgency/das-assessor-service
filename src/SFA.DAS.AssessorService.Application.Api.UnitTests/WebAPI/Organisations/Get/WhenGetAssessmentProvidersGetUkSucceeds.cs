﻿//namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller
//{
//    using Machine.Specifications;
//    using SFA.DAS.AssessorService.Application.Api.Controllers;
//    using FizzWare.NBuilder;
//    using SFA.DAS.AssessorService.ViewModel.Models;
//    using System.Threading.Tasks;
//    using SFA.DAS.AssessorService.Api.Types;

//    [Subject("AssessorService")]
//    public class WhenGetAssessmentProvidersGetUkSucceeds : WhenGetAssessmentProvidersTestBase
//    {
//        private static Organisation _organisationQueryViewModel;
      
//        Establish context = () =>
//        {
//            Setup();

//            _organisationQueryViewModel = Builder<Organisation>.CreateNew().Build();

//            OrganisationQueryRepositoryMock.Setup(q => q.GetByUkPrn(Moq.It.IsAny<int>()))
//                .Returns(Task.FromResult((_organisationQueryViewModel)));

//            OrganisationContoller = new OrganisationQueryController(
//                OrganisationQueryRepositoryMock.Object, 
//                StringLocalizer.Object, 
//                UkPrnValidator,
//                Logger.Object);
//        };

//        Because of = () =>
//        {
//            Result = OrganisationContoller.Get(10000000).Result;
//        };

//        Machine.Specifications.It verify_succesfully = () =>
//        {
//            var result = Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
//            result.Value.Equals(_organisationQueryViewModel);
//        };
//    }
//}
