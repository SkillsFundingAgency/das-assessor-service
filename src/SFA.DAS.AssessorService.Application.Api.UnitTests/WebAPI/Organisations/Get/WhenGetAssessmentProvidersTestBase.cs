namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller
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

    public class WhenGetAssessmentProvidersTestBase
    {     
        protected static Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;
        protected static Mock<IStringLocalizer<OrganisationController>> StringLocalizer;
        protected static IActionResult Result;
        protected static UkPrnValidator UkPrnValidator;
        protected static Mock<ILogger<OrganisationQueryController>> Logger;
        protected static Mock<IMediator> Mediator;

        protected static OrganisationQueryController OrganisationContoller;

        protected static void Setup()
        {          
            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            StringLocalizer = new Mock<IStringLocalizer<OrganisationController>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            StringLocalizer.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<int>()]).Returns(localizedString);

            var ukPrnStringLocalizer = new Mock<IStringLocalizer<UkPrnValidator>>();
            key = ResourceMessageName.NoAssesmentProviderFound;
            localizedString = new LocalizedString(key, "10000000");
            ukPrnStringLocalizer.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<string>()]).Returns(localizedString);

            Logger = new Mock<ILogger<OrganisationQueryController>>();
            Mediator = new Mock<IMediator>();

            UkPrnValidator = new UkPrnValidator(ukPrnStringLocalizer.Object);
        }
    }
}
