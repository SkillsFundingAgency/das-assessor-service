using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class ContactsPrivilege
    {
        public Guid ContactId { get; set; }
        public Guid PrivilegeId { get; set; }
        
        public virtual Privilege Privilege { get; set; }

        public virtual Contact Contact { get; set; }

    }
}
