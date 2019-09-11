using SFA.DAS.AssessorService.Api.Types.Models.OppFinder;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.ViewModels.OppFinder
{
    public class OppFinderPaginationViewModel
    {
        public PaginatedList<OppFinderSearchResult> Standards { get; set; }

        public int PageIndex { get; set; }

        public string ChangePageAction { get; set; }
        public string ChangePageSetAction { get; set; }

        public string Fragment { get; set; }

    }
}
