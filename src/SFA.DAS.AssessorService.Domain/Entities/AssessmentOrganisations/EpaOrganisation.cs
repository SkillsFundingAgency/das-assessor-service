using System;

namespace SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations
{
    public class EpaOrganisation
    {
        public Guid Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public string EndPointAssessorName {get; set;}
        public int OrganisationTypeId { get; set; }
       
        public int? EndPointAssessorUkprn { get; set; }
           public string Status { get; set; }

        public OrganisationData OrganisationData { get; set; }

    }
}