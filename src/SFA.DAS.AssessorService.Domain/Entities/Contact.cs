namespace SFA.DAS.AssessorService.Domain.Entities
{
    using System;
    using Enums;

    public class Contact : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        public string EndPointAssessorOrganisationId { get; set; }

        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
     
        public ContactStatus ContactStatus { get; set; }
    }
}
