namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class AzureSubscriptionResponse
    {
        [JsonProperty("value")]
        public IEnumerable<AzureSubscription> Subscriptions { get; set; }
        [JsonProperty("count")]
        public int TotalCount { get; set; }
        public string NextLink { get; set; }
    }
}
