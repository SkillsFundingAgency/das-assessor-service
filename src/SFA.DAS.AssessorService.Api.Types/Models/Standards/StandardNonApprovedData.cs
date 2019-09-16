using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    public class StandardNonApprovedData
    {
        public int? Level { get; set; }
        public string Category { get; set; }
        public string IfaStatus { get; set; }
        public string IntegratedDegree { get; set; }
        public int? Duration { get; set; }
        public int? MaxFunding { get; set; }
        public string Trailblazer { get; set; }
        public string OverviewOfRole { get; set; }
        public string StandardPageUrl { get; set; }
    }
}
