using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations
{
    public class OrganisationTestBase
    {        
        protected static Mock<IOrganisationRepository> OrganizationRepository;
        protected static Mock<IStringLocalizer<OrganisationController>> OrganisationControllerLocaliser;
        protected static UkPrnValidator UkPrnValidator;
        protected static IActionResult Result;
     
        protected static Mock<ILogger<OrganisationController>> Logger;
        protected static Mock<IMediator> Mediator;
        protected static OrganisationController OrganisationContoller;

        private MockStringLocaliserBuilder _mockStringLocaliserBuilder;

        protected  void Setup()
        {
            OrganizationRepository = new Mock<IOrganisationRepository>();
            Logger = new Mock<ILogger<OrganisationController>>();
            Mediator = new Mock<IMediator>();

            _mockStringLocaliserBuilder = new MockStringLocaliserBuilder();
            OrganisationControllerLocaliser = _mockStringLocaliserBuilder.Build<OrganisationController>();

            var ukPrnStringLocalizer = _mockStringLocaliserBuilder.Build<UkPrnValidator>();     
            UkPrnValidator = new UkPrnValidator(ukPrnStringLocalizer.Object);
        }
    }
}
