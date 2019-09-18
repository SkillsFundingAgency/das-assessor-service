using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderApprovedStandardsResponse
    {
        public PaginatedList<OppFinderApprovedSearchResult> Standards { get; set; }
    }
}
