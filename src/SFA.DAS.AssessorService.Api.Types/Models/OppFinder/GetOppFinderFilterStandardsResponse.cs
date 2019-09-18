using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderFilterStandardsResponse
    {
        public Dictionary<string, int> SectorFilterResults { get; set; }
        public Dictionary<int, int> LevelFilterResults { get; set; }
    }
}
