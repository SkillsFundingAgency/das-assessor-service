using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Data.Entities
{
    public class Contact
    {
        public Guid Id { get; set; }

        public Guid? OrganisationId { get; set; }

        public string EndPointAssessorOrganisationId { get; set; }

        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public string Status { get; set; }
        public Guid? SignInId { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public string SignInType { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
