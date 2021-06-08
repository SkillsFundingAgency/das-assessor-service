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
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public IEnumerable<string> Options { get; set; }
        public bool EPAChanged { get; set; }
        public string StandardPageUrl { get; set; }
        public string VersionStatus { get; set; }
        public string ApplicationStatus { get; set; }

        public static implicit operator StandardVersion(Standard standard)
        {
            return new StandardVersion
            {
                StandardUId = standard.StandardUId,
                Title = standard.Title,
                Version = standard.Version?.ToString() ?? string.Empty,
                IFateReferenceNumber = standard.IfateReferenceNumber,
                LarsCode = standard.LarsCode,
                Level = standard.Level,
                EffectiveFrom = standard.EffectiveFrom.GetValueOrDefault(),
                EffectiveTo = standard.EffectiveTo,
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
                Version = standard.Version.ToString(),
                IFateReferenceNumber = standard.IFateReferenceNumber,
                LarsCode = standard.LarsCode,
                Level = standard.Level,
                EffectiveFrom = standard.LarsEffectiveFrom.GetValueOrDefault(),
                EffectiveTo = standard.LarsEffectiveTo,
                EPAChanged = standard.EPAChanged,
                StandardPageUrl = standard.StandardPageUrl,
                ApplicationStatus = standard.ApplicationStatus
            };
        }
    }
}
