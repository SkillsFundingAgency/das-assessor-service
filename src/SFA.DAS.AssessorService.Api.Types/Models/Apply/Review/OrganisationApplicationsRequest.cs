using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class OrganisationApplicationsRequest : ApplicationsRequest, IRequest<PaginatedList<ApplicationSummaryItem>>
    {
        public OrganisationApplicationsRequest(string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex, int pageSetSize)
            : base(reviewStatus, sortColumn, sortAscending, pageSize, pageIndex, pageSetSize)
        {
        }
    }
}
