using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Orchestrators;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrchestratorTests.LoginOrchestrator
{
    [TestFixture]
    public class WhenRoleIsInvalid : LoginOrchestratorTestBase
    {
        [Test]
        public void ThenInvalidRoleResponseIsReturned()
        {
            var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>() { new ClaimsIdentity(new List<Claim>()) });

            var result = LoginOrchestrator.Login(claimsPrincipal).Result;

            result.Should().Be(LoginResult.InvalidRole);
        }
    }
}