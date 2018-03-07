using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface ISearchApiClient
    {
        Task<IEnumerable<SearchResult>> Search(SearchQuery searchQuery);
    }
}