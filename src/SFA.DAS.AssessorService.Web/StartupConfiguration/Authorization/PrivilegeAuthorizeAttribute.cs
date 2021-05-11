using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class PrivilegeAuthorizeAttribute : AuthorizeAttribute
    {
        public const string POLICY_PREFIX = "PrivilegePolicy_";

        public PrivilegeAuthorizeAttribute(string permission)
        {
            Policy = $"{POLICY_PREFIX}{permission}";
        }
    }
}