using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandardPeriod
    {
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
