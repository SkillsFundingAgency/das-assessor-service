using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class AssessmentOrganisationSummary
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public long? Ukprn { get; set; }
        public OrganisationData OrganisationData { get; set; }

        public int? OrganisationTypeId { get; set; }
        public string OrganisationType { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }

        public class EqualityComparer : IEqualityComparer<AssessmentOrganisationSummary>
        {
            public bool Equals(AssessmentOrganisationSummary x, AssessmentOrganisationSummary y)
            {
                return y != null && (x != null && x.Id == y.Id);
            }

            public int GetHashCode(AssessmentOrganisationSummary obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
