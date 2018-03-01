using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Api.Types;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    using Api.Types.Models;


    public class OrganisationControllerTestBase
    {
        protected static OrganisationController OrganisationController;
        //protected static Mock<IOrganisationService> OrganisationService;
        protected static Mock<ITokenService> TokenService;

        protected static Mock<IOrganisationsApiClient> ApiClient;

        public static void Setup()
        {
            //OrganisationService = new Mock<IOrganisationService>();
            //OrganisationService
            //   .Setup(serv => serv.GetOrganisation("jwt", 12345))
            //   .Returns(Task.FromResult(new Organisation() { Id = "ID1" }));

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

            TokenService = new Mock<ITokenService>();
            TokenService.Setup(s => s.GetToken()).Returns("jwt");

            ApiClient = new Mock<IOrganisationsApiClient>();
            ApiClient.Setup(c => c.Get("12345")).ReturnsAsync(new Organisation() { });

            OrganisationController = new OrganisationController(logger.Object, httpContext.Object, ApiClient.Object);
        }
    }
}
