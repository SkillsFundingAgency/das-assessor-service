namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using SFA.DAS.AssessorService.Domain.Enums;
    using System;

    public class OrganisationUpdateDomainModel
    {
        public Guid Id { get; set; }
     
        public string EndPointAssessorName { get; set; }
        public Guid? PrimaryContactId { get; set; }
        public OrganisationStatus OrganisationStatus { get; set; }
    }
}
