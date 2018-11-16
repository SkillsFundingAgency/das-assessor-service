using System;

namespace SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types
{
    public class StandardData
    {
        public int? Level { get; set; }
        public string Category { get; set; }
        public string IfaStatus { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? LastDateForNewStarts { get; set; }
        public bool IfaOnly { get; set; }

        public int? Duration { get; set; }
        public int? MaxFunding { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool? IsPublished { get; set; }
    }
}