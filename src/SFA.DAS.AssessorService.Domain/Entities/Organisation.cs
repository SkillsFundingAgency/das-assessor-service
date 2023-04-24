using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Organisation : BaseEntity
    {
        public Guid Id { get; set; }

        public string EndPointAssessorOrganisationId { get; set; }
        public int? EndPointAssessorUkprn { get; set; }
        public string EndPointAssessorName { get; set; }

        public string PrimaryContact { get; set; }

        public bool ApiEnabled { get; set; }
        public string ApiUser { get; set; }

        public string Status { get; set; }

        public OrganisationData OrganisationData { get; set; }

        public int? OrganisationTypeId { get; set; }

        public virtual OrganisationType OrganisationType { get; set; }

        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

        public virtual ICollection<OrganisationStandard> OrganisationStandards { get; set; } = new List<OrganisationStandard>();
    }
}