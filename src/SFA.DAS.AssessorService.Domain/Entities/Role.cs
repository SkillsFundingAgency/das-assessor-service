using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Role
    {
       public Guid Id { get; set; }
       public string UserRole { get; set; }

       public IList<ContactsRole> ContactsRoles { get; set; }
    }
}
