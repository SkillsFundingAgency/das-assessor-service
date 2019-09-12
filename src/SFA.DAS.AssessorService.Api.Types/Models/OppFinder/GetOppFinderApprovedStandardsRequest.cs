using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderApprovedStandardsRequest : IRequest<GetOppFinderApprovedStandardsResponse>
    {
        public GetOppFinderApprovedStandardsRequest(string sortColumn, int sortAscending, int pageSize, int? pageIndex, int pageSetSize)
        {
            SortColumn = sortColumn;
            SortAscending = sortAscending;
            PageSize = pageSize;
            PageIndex = pageIndex;
            PageSetSize = pageSetSize;
        }

        public string SortColumn { get; private set; }
        public int SortAscending { get; private set; }
        public int PageSize { get; private set; }
        public int? PageIndex { get; private set; }
        public int PageSetSize { get; private set; }
    }
}
