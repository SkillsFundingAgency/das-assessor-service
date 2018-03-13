using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Orchestrators.Search
{
    public class SearchOrchestrator : ISearchOrchestrator
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ISearchApiClient _searchApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public SearchOrchestrator(ILogger<SearchController> logger, ISearchApiClient searchApiClient, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _searchApiClient = searchApiClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<SearchViewModel> Search(SearchViewModel vm)
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var result = await _searchApiClient.Search(new SearchQuery() { Surname = vm.Surname, Uln = long.Parse(vm.Uln), UkPrn = ukprn, Username = username });
            vm.SearchResults = result.Results;

            return vm;
        }
    }
}