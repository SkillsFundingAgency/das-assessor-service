using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class GetStandardsExportListResponse
    {
        [JsonProperty("courses")]
        public IEnumerable<StandardDetailResponse> Standards { get; set; }
    }

}