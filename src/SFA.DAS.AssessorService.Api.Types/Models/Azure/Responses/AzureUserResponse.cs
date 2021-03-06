﻿namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class AzureUserResponse
    {
        [JsonProperty("value")]
        public IEnumerable<AzureUser> Users { get; set; }
        [JsonProperty("count")]
        public int TotalCount { get; set; }
        public string NextLink { get; set; }
    }
}
