using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrchestratorTests.LoginOrchestrator
{
    [TestFixture]
    public class When_user_has_no_role_claims
    {
        [Test]
        public void Then_orchestrator_does_not_error()
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();

            contextAccessor.SetupGet(a => a.HttpContext.User.Claims).Returns(new List<Claim>
            {
                new Claim("http://schemas.portal.com/ukprn", "12345678"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "username"),
                new Claim("http://schemas.portal.com/mail", "email@domain.com"),
                new Claim("http://schemas.portal.com/displayname", "Mr Jones")
            });

            var organisationsApiClient = new Mock<IOrganisationsApiClient>();
            var loginApiClient = new Mock<ILoginApiClient>();
            var loginOrchestrator = new Orchestrators.Login.LoginOrchestrator(new Mock<ILogger<Orchestrators.Login.LoginOrchestrator>>().Object, contextAccessor.Object,
                loginApiClient.Object);

            loginOrchestrator.Login().Wait();


        }
    }
}