using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class OrganisationStandardVersion
    {
        public string StandardUId { get; set; }

        public decimal? Version { get; set; }

        public int OrganisationStandardId { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateVersionApproved { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }

        public OrganisationStandard OrganisationStandard { get; set; }
    }
}