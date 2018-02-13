namespace SFA.DAS.AssessorService.Data.Entitites
{
    using System;
    using System.Collections.Generic;

    public class Organisation : BaseEntity
    {
        public Guid Id { get; set; }

        public string EndPointAssessorOrganisationId { get; set; }
        public string EndPointAssessorName { get; set; }
        public string Status { get; set; }

        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();      
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
