using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderApprovedStandardDetailsResponse
    {
        public string Title { get; set; }
        public string OverviewOfRole { get; set; }
        public string StandardLevel { get; set; }
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public int TotalActiveApprentices { get; set; }
        public int TotalCompletedAssessments { get; set; }
        public string Sector { get; set; }
        public int? TypicalDuration { get; set; }
        public string ApprovedForDelivery { get; set; }
        public string MaxFunding { get; set; }
        public string Trailblazer { get; set; }
        public string StandardPageUrl { get; set; }
        public string EqaProvider { get; set; }
        public string EqaProviderLink { get; set; }
        public List<OppFinderApprovedStandardDetailsRegionResult> RegionResults { get; set; }
        public List<OppFinderApprovedStandardDetailsVersionResult> VersionResults { get; set; }
    }
}
