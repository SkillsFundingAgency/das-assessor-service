namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Contoller.Put
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Controllers;
    using System.Threading;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;

    [Subject("AssessorService")]
    public class WhenUpdateAssessmentUserSucceeds : WhenUpdateAssessmentUserTestBase
    {
        private static UpdateContactRequest _contactUpdateViewModel;
        private static Contact _contactQueryViewModel;

        Establish context = () =>
        {
            Setup();

            _contactQueryViewModel = Builder<Contact>.CreateNew().Build();

            Mediator.Setup(q => q.Send(Moq.It.IsAny<UpdateContactRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((_contactQueryViewModel)));

            _contactUpdateViewModel = Builder<UpdateContactRequest>.CreateNew()                   
                    .Build();

            ContactContoller = new ContactController(
                  Mediator.Object,
                  StringLocalizer.Object,                 
                  Logger.Object);
        };

        Because of = () =>
        {
            Result = ContactContoller.UpdateContact(_contactUpdateViewModel).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var result = Result as Microsoft.AspNetCore.Mvc.NoContentResult;
            result.Should().NotBeNull();
        };
    }
}
