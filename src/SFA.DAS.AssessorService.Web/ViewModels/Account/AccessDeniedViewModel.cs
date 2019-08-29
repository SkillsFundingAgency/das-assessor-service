using System;

namespace SFA.DAS.AssessorService.Web.ViewModels.Account
{
    public class AccessDeniedViewModel
    {
        public string Title { get; set; }
        public string[] Rights { get; set; }
        public Guid PrivilegeId { get; set; }
        public Guid ContactId { get; set; }
        public bool UserHasUserManagement { get; set; }
        public string ReturnController { get; set; }
        public string ReturnAction { get; set; }
    }
}