namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using System;

    public class OrganisationQueryViewModel
    {
        public Guid Id { get; set; }

        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUKPRN { get; set; }
        public string EndPointAssessorName { get; set; }
        public int? PrimaryContactId { get; set; }

        public string Status { get; set; }
    }
}
