using System;

namespace SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types
{
    public class Period
    {
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}