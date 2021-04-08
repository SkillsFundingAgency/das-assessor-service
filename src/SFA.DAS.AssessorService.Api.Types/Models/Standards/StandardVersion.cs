using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    public class StandardVersion
    {
        public string StandardUId { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string IFateReferenceNumber { get; set; }
        public int Level { get; set; }
        public DateTime EffectiveFrom { get; set; }

        public static implicit operator StandardVersion(Standard standard)
        {
            return new StandardVersion
            {
                StandardUId = standard.StandardUId,
                Title = standard.Title,
                Version = standard.Version?.ToString() ?? string.Empty,
                IFateReferenceNumber = standard.IfateReferenceNumber,
                Level = standard.Level,
                EffectiveFrom = standard.EffectiveFrom.GetValueOrDefault()
            };
        }
    }
}
