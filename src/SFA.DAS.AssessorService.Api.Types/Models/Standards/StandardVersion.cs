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
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public IEnumerable<string> Options { get; set; }
        public string StandardPageUrl { get; set; }

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
                EffectiveTo = standard.EffectiveTo
            };
        }
    }
}
