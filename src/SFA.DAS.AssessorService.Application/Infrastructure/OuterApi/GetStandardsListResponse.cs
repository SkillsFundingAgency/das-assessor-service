using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class GetStandardsListResponse
    {
        [JsonProperty("courses")]
        public IEnumerable<GetStandardsListItem> Standards { get; set; }
    }

    public class GetStandardsListItem
    {
        [JsonProperty("id")]
        public int Id { get ; set ; }
        [JsonProperty("title")]
        public string Title { get ; set ; }
        [JsonProperty("standardDates")]
        public StandardDates StandardDates { get ; set ; }
        [JsonProperty("level")]
        public int Level { get ; set ; }
        [JsonProperty("typicalDuration")]
        public int TypicalDuration { get ; set ; }
        [JsonProperty("maxFunding")]
        public int MaxFunding { get ; set ; }
        [JsonProperty("isActive")]
        public bool IsActive { get ; set ; }
        
    }

    public class StandardDates
    {
        [JsonProperty("lastDateStarts")]
        public DateTime? LastDateStarts { get; set; }

        [JsonProperty("effectiveTo")]
        public DateTime? EffectiveTo { get; set; }

        [JsonProperty("effectiveFrom")]
        public DateTime EffectiveFrom { get; set; }
    }
}