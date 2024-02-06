using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class MergeLogEntry
    {
        public int Id { get; set; }
        public string PrimaryEndPointAssessorOrganisationName { get; set; }
        public string SecondaryEndPointAssessorOrganisationName { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime SecondaryEpaoEffectiveTo { get; set; }
    }
}
