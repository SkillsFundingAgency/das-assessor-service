using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderNonApprovedStandardsRequest : IRequest<GetOppFinderNonApprovedStandardsResponse>
    {
        public GetOppFinderNonApprovedStandardsRequest(string sortColumn, int sortAscending, int pageSize, int? pageIndex, int pageSetSize, string nonApprovedType)
        {
            SortColumn = sortColumn;
            SortAscending = sortAscending;
            PageSize = pageSize;
            PageIndex = pageIndex;
            PageSetSize = pageSetSize;
            NonApprovedType = nonApprovedType;
        }

        public string SortColumn { get; private set; }
        public int SortAscending { get; private set; }
        public int PageSize { get; private set; }
        public int? PageIndex { get; private set; }
        public int PageSetSize { get; private set; }
        public string NonApprovedType { get; private set; }
    }
}
