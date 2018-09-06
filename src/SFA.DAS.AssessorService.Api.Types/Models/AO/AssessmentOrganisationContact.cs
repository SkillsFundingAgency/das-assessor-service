using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class AssessmentOrganisationContact
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string OrganisationId { get; set; }
        public string Status { get; set; }
        public DateTime? UpdatedAt {get; set;}
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPrimaryContact { get; set; }
    }
}
