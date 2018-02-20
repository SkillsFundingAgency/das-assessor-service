namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using System;

    public class OrganisationUpdateDomainModel
    {
        public Guid Id { get; set; }
     
        public string EndPointAssessorName { get; set; }
        public Guid? PrimaryContactId { get; set; }
        public string Status { get; set; }
    }
}
