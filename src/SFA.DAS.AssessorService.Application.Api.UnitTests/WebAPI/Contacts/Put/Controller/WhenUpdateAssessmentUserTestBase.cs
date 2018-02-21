namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Contoller.Put
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Moq;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Controllers;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;

    public class WhenUpdateAssessmentUserTestBase
    {
        protected static Mock<IContactRepository> ContactRepository;
        protected static Mock<IStringLocalizer<ContactController>> StringLocalizer;
        protected static IActionResult Result;
        protected static UkPrnValidator UkPrnValidator;
        protected static Mock<ILogger<ContactController>> Logger;
        protected static Mock<IMediator> Mediator;

        protected static ContactController ContactContoller;

        protected static void Setup()
        {
            ContactRepository = new Mock<IContactRepository>();

            StringLocalizer = new Mock<IStringLocalizer<ContactController>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            StringLocalizer.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<int>()]).Returns(localizedString);
         

            Logger = new Mock<ILogger<ContactController>>();
            Mediator = new Mock<IMediator>();            
        }
    }
}
