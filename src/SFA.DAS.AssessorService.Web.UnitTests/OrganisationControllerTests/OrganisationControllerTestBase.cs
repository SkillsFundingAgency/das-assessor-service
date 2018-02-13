namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Moq;
    using SFA.DAS.AssessorService.Web.Controllers;
    using SFA.DAS.AssessorService.Web.Infrastructure;
    using SFA.DAS.AssessorService.Web.Services;
    using SFA.DAS.AssessorService.Web.ViewModels;

    public class OrganisationControllerTestBase
    {
        protected static OrganisationController OrganisationController;
        protected static Mock<IOrganisationService> OrganisationService;
        protected static Mock<ITokenService> TokenService;       

        public static void  Setup()
        {
            OrganisationService = new Mock<IOrganisationService>();
            OrganisationService
               .Setup(serv => serv.GetOrganisation("jwt"))
               .Returns(Task.FromResult(new Organisation() { Id = "ID1" }));

            var httpContext = new Mock<IHttpContextAccessor>();
            httpContext
                .Setup(c => c.HttpContext)
                .Returns(new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim("ukprn", "12345"),
                        new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "userid1")
                    }))
                });

            var logger = new Mock<ILogger<OrganisationController>>();

            TokenService = new Mock<ITokenService>();            
            TokenService.Setup(s => s.GetJwt()).Returns("jwt");

            OrganisationController = new OrganisationController(OrganisationService.Object, logger.Object, TokenService.Object);
        }
    }
}
