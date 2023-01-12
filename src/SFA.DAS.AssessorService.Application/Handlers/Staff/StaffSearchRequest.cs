using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StaffSearchRequest : IRequest<StaffSearchResult>
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