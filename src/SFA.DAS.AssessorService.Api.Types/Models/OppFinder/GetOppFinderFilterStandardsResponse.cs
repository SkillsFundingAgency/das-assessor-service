using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderFilterStandardsResponse
    {
        public List<OppFinderFilterResult> SectorFilterResults { get; set; }
        public List<OppFinderFilterResult> LevelFilterResults { get; set; }
    }
}
