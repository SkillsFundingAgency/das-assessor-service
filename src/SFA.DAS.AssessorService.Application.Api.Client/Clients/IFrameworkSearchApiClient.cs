using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IFrameworkSearchApiClient
    {
        Task<List<FrameworkSearchResult>> FrameworkSearch(FrameworkSearchQuery searchQuery);
    }
}