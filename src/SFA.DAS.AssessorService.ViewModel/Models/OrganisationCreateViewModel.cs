namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using System;

    public class OrganisationCreateViewModel
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUKPRN { get; set; }
        public string EndPointAssessorName { get; set; }
        public int? PrimaryContactId { get; set; }
    }
}
