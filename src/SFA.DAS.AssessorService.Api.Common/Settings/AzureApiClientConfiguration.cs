﻿using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Api.Common
{
    public class AzureApiClientConfiguration : IAzureApiClientConfiguration
    {
        [JsonRequired] public string Id { get; set; }

        [JsonRequired] public string Key { get; set; }

        [JsonRequired] public string ApiBaseAddress { get; set; }

        [JsonRequired] public string ProductId { get; set; }

        [JsonRequired] public string GroupId { get; set; }

        [JsonRequired] public string RequestBaseAddress { get; set; }
    }
}