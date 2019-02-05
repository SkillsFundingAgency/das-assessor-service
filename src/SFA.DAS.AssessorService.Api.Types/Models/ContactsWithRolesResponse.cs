using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class ContactsWithRolesResponse
    {
        public ContactsWithRolesResponse()
        {
            Roles = new List<string>();
        }
        public Guid Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; }
    }
}
