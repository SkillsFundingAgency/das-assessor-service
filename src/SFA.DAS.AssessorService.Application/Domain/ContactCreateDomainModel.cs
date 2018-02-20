namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using SFA.DAS.AssessorService.Domain.Enums;
    using System;

    public class ContactCreateDomainModel
    {      
        public Guid OrganisationId { get; set; }

        public int EndPointAssessorContactId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUKPRN { get; set; }

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public ContactStatus ContactStatus { get; set; }
    }
}
