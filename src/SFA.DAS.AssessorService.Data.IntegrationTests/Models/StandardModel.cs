
using Moq;
using NUnit.Framework.Constraints;
using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class StandardModel : TestModel
    {
        public string StandardUId { get; set; }
        public string IFateReferenceNumber { get; set; }
        public int LarsCode { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public int Level { get; set; }
        public string Status { get; set; }
        public int TypicalDuration { get; set; }
        public int MaxFunding { get; set; }
        public int IsActive { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime VersionApprovedForDelivery { get; set; }
        public int ProposedTypicalDuration { get; set; }
        public int ProposedMaxFunding { get; set; }
        public bool EPAChanged { get; set; }
        public string StandardPageUrl { get; set; }
        public string TrailblazerContact { get; set; }
        public string Route { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }
        public string OverviewOfRole { get; set; }
    }
}
