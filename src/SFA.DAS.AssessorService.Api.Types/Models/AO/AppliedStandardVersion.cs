using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class AppliedStandardVersion
    {
        public string ApprovedStatus { get; set; }
        public Guid ApplicationId { get; set; }
        public string StandardUId { get; set; }
        public string Title { get; set; }
        public string IFateReferenceNumber { get; set; }
        public decimal Version { get; set; }
        public int Level { get; set; }
        public int LarsCode { get; set; }
        public string Status { get; set; }
        public string ApplicationStatus { get; set; }
        public bool EPAChanged { get; set; }
        public string StandardPageUrl { get; set; }
        public DateTime? LarsEffectiveFrom { get; set; }
        public DateTime? LarsEffectiveTo { get; set; }
        public DateTime? VersionEarliestStartDate { get; set; }
        public DateTime? VersionLatestStartDate { get; set; }
        public DateTime? VersionLatestEndDate { get; set; }
        public DateTime? StdEffectiveFrom { get; set; }
        public DateTime? StdEffectiveTo { get; set; }
        public DateTime? StdVersionEffectiveFrom { get; set; }
        public DateTime? StdVersionEffectiveTo { get; set; }
    }
}