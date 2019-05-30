using System;

namespace SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations
{
    // TODO: This looks a lot like SFA.DAS.AssessorService.Domain.Entities.Organisation
    public class EpaOrganisation
    {
        public Guid Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public string EndPointAssessorName {get; set;}
        public int OrganisationTypeId { get; set; }
        public int? EndPointAssessorUkprn { get; set; }
        public string Status { get; set; }
        public string PrimaryContact { get; set; }
        public OrganisationData OrganisationData { get; set; }
    }
}