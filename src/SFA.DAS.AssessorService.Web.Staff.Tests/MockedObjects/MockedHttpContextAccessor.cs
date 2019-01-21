using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.MockedObjects
{
    public class MockedHttpContextAccessor
    {
        public static Mock<IHttpContextAccessor> Setup()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "jcoxhead")
            }));

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext { User = user };

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            return mockHttpContextAccessor;
        }
    }
}
