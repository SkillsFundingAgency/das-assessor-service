using System;

namespace SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types
{
    public class Standard
    {
        public string StandardId { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public DateTime EffectiveFrom { get; set; }
    }
}