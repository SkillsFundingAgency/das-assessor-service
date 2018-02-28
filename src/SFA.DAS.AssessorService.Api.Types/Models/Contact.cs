namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using System;
    using Domain.Enums;

    public class Contact
    {
        public Guid Id { get; set; }
        public Guid OrganisationId { get; set; }

        public int EndPointAssessorContactId { get; set; }      

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public ContactStatus ContactStatus { get; set; }
    }
}
