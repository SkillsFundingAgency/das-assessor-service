using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners
{
    public class LearnerDetailForExternalApi
    {
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public DateTime LearnerStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public int? CompletionStatus { get; set; }

        public StandardCollation Standard { get; set; }
        
        public string EndPointAssessorOrganisationId { get; set; }
        public int UkPrn { get; set; }
        public string OrganisationName { get; set; }
    }
}
