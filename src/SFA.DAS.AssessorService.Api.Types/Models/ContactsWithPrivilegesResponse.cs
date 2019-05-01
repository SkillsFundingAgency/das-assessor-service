using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class ContactsWithPrivilegesResponse
    {
        public ContactsWithPrivilegesResponse()
        {
            Privileges = new List<string>();
        }
        public Contact Contact { get; set; }
        public List<string> Privileges { get;  }
    }
}
