using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<Privilege> AllPrivilegeTypes { get; set; }

        public EditPrivilegesViewModel EditPrivilegesViewModel {
            get { return new EditPrivilegesViewModel()
            {
                ContactId = Id, 
                PrivilegeViewModels = AllPrivilegeTypes.Select(pt => new PrivilegeViewModel()
                {
                    Privilege = pt,
                    Selected = AssignedPrivileges.Any(ap => ap.PrivilegeId == pt.Id)
                }).ToArray()
            }; }
        }

        public bool ContactIsAdmin(Guid privilegeId)
        {
            return AssignedPrivileges.Any(ap => ap.Privilege.MustBeAtLeastOneUserAssigned) && !AllPrivilegeTypes.Single(t => t.Id == privilegeId).MustBeAtLeastOneUserAssigned;
        }
    }
}