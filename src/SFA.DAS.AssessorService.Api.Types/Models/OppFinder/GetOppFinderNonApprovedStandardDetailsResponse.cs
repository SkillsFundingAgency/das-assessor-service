using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderNonApprovedStandardDetailsResponse
    {
        public string Title { get; set; }
        public string OverviewOfRole { get; set; }
        public string StandardLevel { get; set; }
        public string StandardReference { get; set; }
        public string Sector { get; set; }
        public int TypicalDuration { get; set; }
        public string Trailblazer { get; set; }
        public string StandardPageUrl { get; set; }
    }
}
