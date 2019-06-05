using System;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels
{
    public class UserViewModel
    {
        private string _actionRequired;
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }

        public string ActionRequired
        {
            get => _actionRequired ?? (this.Status == ContactStatus.Active ? "No action required" : "");
            set => _actionRequired = value;
        }
        
        public List<ContactsPrivilege> AssignedPrivileges { get; set; }
    }
}