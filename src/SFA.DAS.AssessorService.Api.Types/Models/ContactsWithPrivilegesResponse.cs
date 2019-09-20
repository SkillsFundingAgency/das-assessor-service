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
            Privileges = new List<PrivilegeResponse>();
        }
        public ContactResponse Contact { get; set; }
        public List<PrivilegeResponse> Privileges { get;  }
    }
}
