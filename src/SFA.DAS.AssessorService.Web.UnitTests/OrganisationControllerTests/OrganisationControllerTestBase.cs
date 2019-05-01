using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    using Api.Types.Models;


    public class OrganisationControllerTestBase
    {
        protected static OrganisationController OrganisationController;
        //protected static Mock<IOrganisationService> OrganisationService;
        protected static Mock<ITokenService> TokenService;
        protected static Mock<ISessionService> SessionService;
        protected static Mock<IOrganisationsApiClient> ApiClient;

        public static void Setup()
        {         
            var httpContext = new Mock<IHttpContextAccessor>();
            httpContext
                .Setup(c => c.HttpContext)
                .Returns(new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim("ukprn", "12345"),
                        new Claim("http://schemas.portal.com/ukprn", "12345")
                    }))
                });

            var logger = new Mock<ILogger<OrganisationController>>();
            SessionService = new Mock<ISessionService>();
            TokenService = new Mock<ITokenService>();
            TokenService.Setup(s => s.GetToken()).Returns("jwt");

            ApiClient = new Mock<IOrganisationsApiClient>();
            ApiClient.Setup(c => c.Get("12345")).ReturnsAsync(new OrganisationResponse() { });

            OrganisationController = new OrganisationController(logger.Object, httpContext.Object, ApiClient.Object);
        }
    }
}
