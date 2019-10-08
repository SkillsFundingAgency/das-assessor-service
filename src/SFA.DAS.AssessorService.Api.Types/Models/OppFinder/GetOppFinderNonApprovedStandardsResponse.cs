using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderNonApprovedStandardsResponse
    {
        public PaginatedList<OppFinderSearchResult> Standards { get; set; }
    }
}
