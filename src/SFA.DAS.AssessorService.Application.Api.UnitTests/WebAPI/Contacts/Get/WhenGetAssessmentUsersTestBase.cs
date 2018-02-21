namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.ContactContoller
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

    public class WhenGetAssessmentUsersTestBase
    {
        protected static Mock<IContactQueryRepository> ContactQueryRepository;
        protected static Mock<IStringLocalizer<ContactQueryController>> StringLocalizer;
        protected static IActionResult Result;
        protected static UkPrnValidator UkPrnValidator;
        protected static Mock<ILogger<ContactQueryController>> Logger;
        protected static Mock<IMediator> Mediator;

        protected static ContactQueryController ContactQueryController;

        protected static void Setup()
        {
            ContactQueryRepository = new Mock<IContactQueryRepository>();

            StringLocalizer = new Mock<IStringLocalizer<ContactQueryController>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            StringLocalizer.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<int>()]).Returns(localizedString);

            Logger = new Mock<ILogger<ContactQueryController>>();
            Mediator = new Mock<IMediator>();         
        }
    }
}
