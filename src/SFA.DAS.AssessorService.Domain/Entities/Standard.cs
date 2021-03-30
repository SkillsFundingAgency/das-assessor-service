using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Standard
    {
        public string StandardUId { get; set; }
        public string IfateReferenceNumber { get; set; }
        public int LarsCode { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public int Level { get; set; }
        public string Status { get; set; }
        public int TypicalDuration { get; set; }
        public int MaxFunding { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastDateStarts { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? VersionEarliestStartDate { get; set; }
        public DateTime? VersionLatestStartDate { get; set; }
        public DateTime? VersionLatestEndDate { get; set; }
        public DateTime? VersionApprovedForDelivery { get; set; }
        public int ProposedTypicalDuration { get; set; }
        public int ProposedMaxFunding { get; set; }
    }
}
