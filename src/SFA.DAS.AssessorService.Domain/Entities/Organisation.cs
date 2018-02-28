namespace SFA.DAS.AssessorService.Domain.Entities
{
    using SFA.DAS.AssessorService.Domain.Enums;
    using System;
    using System.Collections.Generic;

    public class Organisation : BaseEntity
    {
        public Guid Id { get; set; }

        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUKPRN { get; set; }
        public string EndPointAssessorName { get; set; }

        public Guid? PrimaryContactId { get; set; }

        public OrganisationStatus OrganisationStatus { get; set; }

        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();      
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
