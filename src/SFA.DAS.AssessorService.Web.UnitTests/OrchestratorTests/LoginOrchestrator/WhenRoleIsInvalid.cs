using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new List<ClaimsIdentity>() {new ClaimsIdentity(new List<Claim>())})
            };

            var result = LoginOrchestrator.Login(context).Result;

            result.Should().Be(LoginResult.InvalidRole);
        }
    }
}