namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class AzureProductResponse
    {
        [JsonProperty("value")]
        public IEnumerable<AzureProduct> Products { get; set; }
        [JsonProperty("count")]
        public int TotalCount { get; set; }
        public string NextLink { get; set; }
    }
}
