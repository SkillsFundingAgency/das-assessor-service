using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Privilege
    {
        public Guid Id { get; set; }
        public string UserPrivilege { get; set; }
        public bool MustBeAtLeastOneUserAssigned { get; set; }
        public PrivilegeData PrivilegeData { get; set; }
        public string Key { get; set; }
        public bool Enabled { get; set; }
    }
}