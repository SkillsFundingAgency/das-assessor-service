using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class GetStandardOptionsListResponse
    {
        [JsonProperty("standardOptions")]
        public IEnumerable<GetStandardOptionsItem> StandardOptions { get; set; }
    }

    public class GetStandardOptionsItem
    {
        [JsonProperty("standardUId")]
        public string StandardUId { get; set; }
        [JsonProperty("larsCode")]
        public int LarsCode { get; set; }
        [JsonProperty("ifateReferenceNumber")]
        public string IfateReferenceNumber { get; set; }
        [JsonProperty("options")]
        public List<string> Options { get; set; }

    }
}
