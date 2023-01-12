using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class GetStandardsExportListResponse
    {
        [JsonProperty("courses")]
        public IEnumerable<StandardDetailResponse> Standards { get; set; }
    }

}