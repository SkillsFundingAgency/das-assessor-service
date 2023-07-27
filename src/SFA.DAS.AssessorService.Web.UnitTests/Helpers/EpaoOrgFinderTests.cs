using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Helpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers;

public class EpaoOrgFinderTests
{
    [Test, RecursiveMoqAutoData]
    public void GetFromClaim_InvokesHttpContext_WithClaimType(Mock<IHttpContextAccessor> accessor)
    {
        accessor.Setup(a => a.HttpContext.User.FindFirst(EpaOrgIdFinder.ClaimType))
                .Returns(new Claim("some type", "some value"));

        var result = EpaOrgIdFinder.GetFromClaim(accessor.Object);

        Assert.That(result, Is.EqualTo("some value"));
    }
}
