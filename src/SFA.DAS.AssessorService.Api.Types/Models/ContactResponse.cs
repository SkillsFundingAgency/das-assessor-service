using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class ContactResponse
    {
        public string EndPointAssessorOrganisationId { get; set; }

        public Guid? OrganisationId { get; set; }

        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public string Status { get; set; }
        public Guid Id { get; set; }

        public string PhoneNumber { get; set; }
    }
}
