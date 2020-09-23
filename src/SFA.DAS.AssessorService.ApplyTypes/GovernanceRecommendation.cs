using System;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class GovernanceRecommendation
    {
        public string SelectedRecommendation { get; set; }
        public string ApprovedInternalComments { get; set; }
        public string RejectedInternalComments { get; set; }

        public string RecommendedBy { get; set; }
        public DateTime RecommendedDateTime { get; set; }
    }

    public static class GovernanceRecommendationTypes
    {
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }
}