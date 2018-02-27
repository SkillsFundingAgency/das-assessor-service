﻿namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.ContactContoller
{
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Controllers;
    using FizzWare.NBuilder;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Threading.Tasks;
    using FluentAssertions;

    [Subject("AssessorService")]
    public class WhenGetAssessmentUsersByUserNameAndEmailSucceeds : WhenGetAssessmentUsersTestBase
    {
        private static Contactl _organisationQueryViewModels;
      
        Establish context = () =>
        {
            Setup();

            _organisationQueryViewModels = Builder<Contactl>.CreateNew().Build();

            ContactQueryRepository.Setup(q => q.GetContact(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((_organisationQueryViewModels)));

            ContactQueryController = new ContactQueryController(                
                ContactQueryRepository.Object, 
                Logger.Object);
        };

        Because of = () =>
        {
            Result = ContactQueryController.GetContactsByUserNameAndEmail("TestUser", "xxxxx").Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
            result.Should().NotBeNull();
        };

         Machine.Specifications.It should_be_correct_value  = () =>
         {
             var result = Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
             (result.Value is Contactl).Should().BeTrue();             
         };
    }
}
