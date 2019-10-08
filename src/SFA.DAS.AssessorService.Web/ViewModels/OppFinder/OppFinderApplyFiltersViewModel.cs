using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.ViewModels.OppFinder
{
    public class OppFinderApplyFiltersViewModel
    {
        public string[] SectorFilters { get; set; }

        public string[] LevelFilters { get; set; }
    }
}
