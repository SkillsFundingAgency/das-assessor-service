using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class PrivilegeAuthorizeAttribute : AuthorizeAttribute
    {
        private const string POLICY_PREFIX = "PrivilegePolicy_";

        public PrivilegeAuthorizeAttribute(string permission) => Permission = permission;

        public string Permission
        {
            get => Policy.Substring(POLICY_PREFIX.Length);
            set => Policy = $"{POLICY_PREFIX}{value}";
        }
    }
}