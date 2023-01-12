using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.OppFinder
{
    public class OppFinderShowFiltersViewModel
    {
        public List<OppFinderFilterResult> SectorFilters { get; set; }

        public List<OppFinderFilterResult> LevelFilters { get; set; }
    }
}
