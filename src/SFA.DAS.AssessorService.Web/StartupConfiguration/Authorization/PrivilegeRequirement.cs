using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class PrivilegeRequirement : IAuthorizationRequirement
    {
        public PrivilegeRequirement(string privilege)
        {
            Privilege = privilege;
        }

        public string Privilege { get; }
    }
}