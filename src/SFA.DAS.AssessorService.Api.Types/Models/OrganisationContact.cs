using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class OrganisationContact
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public Guid OrganisationId { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        
    }
}
