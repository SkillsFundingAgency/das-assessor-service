using System;

namespace SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types
{
    public class Standard
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public DateTime EffectiveFrom { get; set; }
    }
}