using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi
{
    public class GetStandardsExportListResponse
    {
        [JsonProperty("courses")]
        public IEnumerable<StandardDetailResponse> Standards { get; set; }
    }

}