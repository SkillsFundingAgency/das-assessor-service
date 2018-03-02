using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Web.Orchestrators;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrchestratorTests.LoginOrchestrator
{
    [TestFixture]
    public class WhenEpaoOrgDoesNotExist : LoginOrchestratorTestBase
    {
        [Test]
        public void ThenNotRegisteredResponseIsReturned()
        {
            var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(new List<Claim> {new Claim("http://schemas.portal.com/service", "EPA")}),
                new ClaimsIdentity(new List<Claim> {new Claim("http://schemas.portal.com/ukprn", "12345")})
            });

            OrganisationsApiClient.Setup(c => c.Get("12345")).Throws<EntityNotFoundException>();

            var result = LoginOrchestrator.Login(claimsPrincipal).Result;

            result.Should().Be(LoginResult.NotRegistered);
        }
    }
}