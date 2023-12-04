using System;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    public class StandardVersion
    {
        public string StandardUId { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string IFateReferenceNumber { get; set; }
        public int LarsCode { get; set; }
        public int Level { get; set; }
        public bool CoronationEmblem { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? LastDateStarts { get; set; }
        public DateTime? VersionEarliestStartDate { get; set; }
        public DateTime? VersionLatestStartDate { get; set; }
        public DateTime? VersionLatestEndDate { get; set; }
        public IEnumerable<string> Options { get; set; }
        public bool EPAChanged { get; set; }
        public string StandardPageUrl { get; set; }

        public static implicit operator StandardVersion(Standard standard)
        {
            return new StandardVersion
            {
                StandardUId = standard.StandardUId,
                Title = standard.Title,
                Version = standard.Version,
                IFateReferenceNumber = standard.IfateReferenceNumber,
                LarsCode = standard.LarsCode,
                Level = standard.Level,
                CoronationEmblem = standard.CoronationEmblem,
                EffectiveFrom = standard.EffectiveFrom,
                EffectiveTo = standard.EffectiveTo,
                LastDateStarts = standard.LastDateStarts,
                VersionEarliestStartDate = standard.VersionEarliestStartDate,
                VersionLatestStartDate = standard.VersionLatestStartDate,
                VersionLatestEndDate = standard.VersionLatestEndDate,
                EPAChanged = standard.EPAChanged,
                StandardPageUrl = standard.StandardPageUrl
            };
        }

        public static implicit operator StandardVersion(AppliedStandardVersion standard)
        {
            return new StandardVersion
            {
                StandardUId = standard.StandardUId,
                Title = standard.Title,
                Version = standard.Version,
                IFateReferenceNumber = standard.IFateReferenceNumber,
                LarsCode = standard.LarsCode,
                Level = standard.Level,
                CoronationEmblem = standard.CoronationEmblem,
                EffectiveFrom = standard.LarsEffectiveFrom,
                EffectiveTo = standard.LarsEffectiveTo,
                VersionEarliestStartDate = standard.VersionEarliestStartDate,
                VersionLatestEndDate = standard.VersionLatestEndDate,
                EPAChanged = standard.EPAChanged,
                StandardPageUrl = standard.StandardPageUrl
            };
        }
    }
}
