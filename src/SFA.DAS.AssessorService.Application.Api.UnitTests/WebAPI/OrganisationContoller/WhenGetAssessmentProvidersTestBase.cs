namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Moq;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Controllers;
    using SFA.DAS.AssessorService.Application.Interfaces;

    public class WhenGetAssessmentProvidersTestBase
    {
        protected static Mock<IOrganisationRepository> OrganizationRepository;
        protected static Mock<IStringLocalizer<OrganisationController>> StringLocalizer;
        protected static IActionResult Result;     

        protected static OrganisationController OrganisationContoller;

        protected static void Setup()
        {
            OrganizationRepository = new Mock<IOrganisationRepository>();

            StringLocalizer = new Mock<IStringLocalizer<OrganisationController>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            StringLocalizer.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<int>()]).Returns(localizedString);
        }
    }
}
