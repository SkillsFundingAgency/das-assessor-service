using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ISearchApiClient _searchApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public SearchController(ILogger<SearchController> logger, ISearchApiClient searchApiClient, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _searchApiClient = searchApiClient;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] SearchViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var result = await _searchApiClient.Search(new SearchQuery() {Surname = vm.Surname, Uln = vm.Uln, UkPrn = ukprn, Username = username});
            vm.SearchResults = result.Results;

            if (result.Results.Any())
            {                
                return View("Results", vm);
            }

            return View("Index", vm);
        }
    }
}