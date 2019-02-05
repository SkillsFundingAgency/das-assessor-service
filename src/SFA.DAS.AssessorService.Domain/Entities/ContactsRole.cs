using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class ContactsRole
    {
        public Guid ContactId { get; set; }
        public Guid RoleId { get; set; }
        
        public virtual Role Role { get; set; }

        public virtual Contact Contact { get; set; }

    }
}
