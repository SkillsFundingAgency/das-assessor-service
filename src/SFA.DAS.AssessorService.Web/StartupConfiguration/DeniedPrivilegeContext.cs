using System;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class DeniedPrivilegeContext
    {
        public Guid PrivilegeId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}