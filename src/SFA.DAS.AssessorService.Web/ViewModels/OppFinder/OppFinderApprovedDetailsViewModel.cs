using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.OppFinder
{
    public class OppFinderApprovedDetailsViewModel : OppFinderDetailsViewModel
    {
        public int StandardCode { get; internal set; }
        public string EqaProvider { get; set; }
        public int TotalActiveApprentices { get; set; }
        public int TotalCompletedAssessments { get; set; }
        public string ApprovedForDelivery { get; set; }
        public string MaxFunding { get; set; }
        public List<OppFinderApprovedStandardDetailsRegionResult> RegionResults { get; set; }
        public List<OppFinderApprovedStandardDetailsVersionResult> VersionResults { get; set; }
    }
}
