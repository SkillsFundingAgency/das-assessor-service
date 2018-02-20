namespace SFA.DAS.AssessorService.ViewModel.Models
{   
    using SFA.DAS.AssessorService.Domain.Enums;
    using System;

    public class OrganisationCreateDomainModel
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUKPRN { get; set; }
        public string EndPointAssessorName { get; set; }
        public Guid? PrimaryContactId { get; set; }

        public OrganisationStatus OrganisationStatus { get; set; }
    }
}
