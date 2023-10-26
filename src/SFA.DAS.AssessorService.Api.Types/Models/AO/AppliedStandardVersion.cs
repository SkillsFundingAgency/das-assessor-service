using SFA.DAS.AssessorService.ApplyTypes;
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
        public string Version { get; set; }
        public int Level { get; set; }
        public bool CoronationEmblem { get; set; }
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
        public ApplyData ApplyData { get; set; }
        public string EqaProviderName { get; set; }
        public string EqaProviderContactName { get; set; }
        public string EqaProviderContactEmail { get; set; }
        public bool EpaoMustBeApprovedByRegulatorBody { get; set; }
    }
}