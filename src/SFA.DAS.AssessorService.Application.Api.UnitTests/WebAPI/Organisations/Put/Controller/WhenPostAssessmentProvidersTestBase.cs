namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller.Put
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

    public class WhenPutAssessmentProvidersTestBase
    {
        protected static Mock<IOrganisationRepository> OrganizationRepository;
        protected static Mock<IStringLocalizer<OrganisationController>> StringLocalizer;
        protected static IActionResult Result;
        protected static UkPrnValidator UkPrnValidator;
        protected static Mock<ILogger<OrganisationController>> Logger;
        protected static Mock<IMediator> Mediator;

        protected static OrganisationController OrganisationContoller;

        protected static void Setup()
        {
            OrganizationRepository = new Mock<IOrganisationRepository>();

            StringLocalizer = new Mock<IStringLocalizer<OrganisationController>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            StringLocalizer.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<int>()]).Returns(localizedString);

            var ukPrnStringLocalizer = new Mock<IStringLocalizer<UkPrnValidator>>();
            key = ResourceMessageName.NoAssesmentProviderFound;
            localizedString = new LocalizedString(key, "10000000");
            ukPrnStringLocalizer.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<string>()]).Returns(localizedString);

            Logger = new Mock<ILogger<OrganisationController>>();
            Mediator = new Mock<IMediator>();

            UkPrnValidator = new UkPrnValidator(ukPrnStringLocalizer.Object);
        }
    }
}
