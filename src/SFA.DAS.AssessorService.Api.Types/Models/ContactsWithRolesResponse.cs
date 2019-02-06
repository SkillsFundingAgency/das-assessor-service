using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class ContactsWithRolesResponse
    {
        public ContactsWithRolesResponse()
        {
            Roles = new List<string>();
        }
        public Contact Contact { get; set; }
        public List<string> Roles { get;  }
    }
}
