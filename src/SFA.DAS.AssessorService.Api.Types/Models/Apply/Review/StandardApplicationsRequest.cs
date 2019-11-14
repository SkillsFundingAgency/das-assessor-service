using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class StandardApplicationsRequest : IRequest<PaginatedList<ApplicationSummaryItem>>
    {
        public string OrgansiationId { get; }
        public string ReviewStatus { get; }
        public string SortColumn { get; }
        public int SortAscending { get; }
        public int PageSize { get; }
        public int PageIndex { get; }
        public int PageSetSize { get; }

        public StandardApplicationsRequest(string organisationId, string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex, int pageSetSize)
        {
            OrgansiationId = organisationId;
            ReviewStatus = reviewStatus;
            SortColumn = sortColumn;
            SortAscending = sortAscending;
            PageSize = pageSize;
            PageIndex = pageIndex;
            PageSetSize = pageSetSize;
        }
    }
}
