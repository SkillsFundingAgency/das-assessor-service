using System;

namespace SFA.DAS.AssessmentOrgs.Api.Client.Core.Types
{
    public class Period
    {
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}