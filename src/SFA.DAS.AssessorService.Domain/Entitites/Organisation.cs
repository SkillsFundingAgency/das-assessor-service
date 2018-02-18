namespace SFA.DAS.AssessorService.Domain.Entities
{
    using System;
    using System.Collections.Generic;

    public class Organisation : BaseEntity
    {
        public Guid Id { get; set; }

        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUKPRN { get; set; }
        public string EndPointAssessorName { get; set; }
        public int? PrimaryContactId { get; set; }


        public string Status { get; set; }

        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();      
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
