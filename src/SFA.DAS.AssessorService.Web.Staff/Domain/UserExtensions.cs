namespace SFA.DAS.AssessorService.Web.Staff.Domain
{
    using System.Linq;
    using System.Security.Claims;

    public static class UserExtensions
    {
        public static string OperatorName(this ClaimsPrincipal user)
        {
            var identity = user.Identities.FirstOrDefault();

            var givenNameClaim = identity.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            var surnameClaim = identity.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");

            var operatorName = $"{givenNameClaim?.Value} {surnameClaim?.Value}";

            return operatorName;
        }
    }
}
