namespace SFA.DAS.AssessorService.Application.Domain
{
    using System;
    using AssessorService.Domain.Enums;

    public class ContactCreateDomainModel
    {      
        public Guid OrganisationId { get; set; }

        public int EndPointAssessorContactId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUkprn { get; set; }

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string Username { get; set; }
        public ContactStatus ContactStatus { get; set; }
    }
}
