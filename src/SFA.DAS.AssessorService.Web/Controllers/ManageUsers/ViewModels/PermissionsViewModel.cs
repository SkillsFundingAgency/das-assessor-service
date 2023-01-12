using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels
{
    public class EditPrivilegesViewModel
    {
        public Guid ContactId { get; set; }

        public PrivilegeViewModel[] PrivilegeViewModels { get; set; }
        public string Button { get; set; }
    }

    public class PrivilegeViewModel
    {
        public Privilege Privilege { get; set; }
        public bool Selected { get; set; }
    }
}