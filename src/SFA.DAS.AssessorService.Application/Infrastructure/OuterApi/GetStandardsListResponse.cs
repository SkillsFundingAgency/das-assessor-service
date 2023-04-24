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
        [JsonProperty("standardUId")]
        public string StandardUId { get; set; }
        [JsonProperty("ifateReferenceNumber")]
        public string IfateReferenceNumber { get; set; }
        [JsonProperty("larsCode")]
        public int LarsCode { get ; set ; }
        [JsonProperty("title")]
        public string Title { get ; set ; }
        [JsonProperty("version")]
        public decimal? Version { get; set; }
        [JsonProperty("standardDates")]
        public StandardDates StandardDates { get ; set ; }
        [JsonProperty("level")]
        public int Level { get ; set ; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("typicalDuration")]
        public int TypicalDuration { get ; set ; }
        [JsonProperty("maxFunding")]
        public int MaxFunding { get ; set ; }
        [JsonProperty("isActive")]
        public bool IsActive { get ; set ; }
    }
}