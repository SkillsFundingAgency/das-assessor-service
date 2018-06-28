using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StaffSearchRequest : IRequest<List<SearchResult>>
    {
        public StaffSearchRequest(string searchQuery)
        {
            SearchQuery = searchQuery;
        }

        public string SearchQuery { get; set; }
    }
}