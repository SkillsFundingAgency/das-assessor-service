using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Orchestrators.Login;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrchestratorTests.LoginOrchestrator
{
    [TestFixture]
    public class WhenEpaoOrgIsDeleted : LoginOrchestratorTestBase
    {
        [Test]
        public void ThenNotRegisteredResponseIsReturned()
        {
            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new List<ClaimsIdentity>()
                {
                    new ClaimsIdentity(new List<Claim> {new Claim("http://schemas.portal.com/service", "EPA")}),
                    new ClaimsIdentity(new List<Claim> {new Claim("http://schemas.portal.com/ukprn", "12345")})
                })
            };

            OrganisationsApiClient.Setup(c => c.Get("12345"))
                .ReturnsAsync(new OrganisationResponse() {Status = OrganisationStatus.Deleted});

            var result = LoginOrchestrator.Login(context).Result;

            result.Should().Be(LoginResult.NotRegistered);
        }
    }
}