using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetMergeOrganisationResponse
    {
        public int Id { get; set; }
        public string PrimaryEndPointAssessorOrganisationId { get; set; }
        public string SecondaryEndPointAssessorOrganisationId { get; set; }
        public DateTime SecondaryEPAOEffectiveTo { get; set; }
        public DateTime? CompletionDate { get; set; }
    }
}
