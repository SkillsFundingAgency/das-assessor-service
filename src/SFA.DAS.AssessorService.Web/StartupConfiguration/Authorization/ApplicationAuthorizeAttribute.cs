using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class ApplicationAuthorizeAttribute : AuthorizeAttribute
    {
        public const string POLICY_PREFIX = "ApplicationPolicy_";

        public ApplicationAuthorizeAttribute(string routeId)
        {
            Policy = $"{POLICY_PREFIX}{routeId}";
        }
    }
}