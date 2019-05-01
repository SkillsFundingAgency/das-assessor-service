using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Privilege
    {
       public Guid Id { get; set; }
       public string UserPrivilege { get; set; }

       public IList<ContactsPrivilege> ContactsPrivileges { get; set; }
    }
}
