using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StaffSearchRequest : IRequest<PaginatedList<StaffSearchResult>>
    {
        public StaffSearchRequest(string searchQuery, int page)
        {
            SearchQuery = searchQuery;
            Page = page;
        }

        public string SearchQuery { get; set; }
        public int Page { get; }
    }
}