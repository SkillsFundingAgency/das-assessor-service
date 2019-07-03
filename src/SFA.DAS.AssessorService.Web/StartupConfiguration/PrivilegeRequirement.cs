using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class PrivilegeRequirement : IAuthorizationRequirement
    {
        public string Privilege { get; }

        public PrivilegeRequirement(string privilege)
        {
            Privilege = privilege;
        }
    }
}