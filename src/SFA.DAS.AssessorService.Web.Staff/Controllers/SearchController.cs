using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ApiClient _apiClient;
        private readonly ISessionService _sessionService;

        public SearchController(ILogger<SearchController> logger, ApiClient apiClient, ISessionService sessionService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm]SearchViewModel searchRequest)
        {
            var searchResults = await _apiClient.Search(searchRequest.SearchString);

            _sessionService.Set("SearchResults", searchResults);

            return RedirectToAction("Results");
        }

        [HttpGet("results")]
        public async Task<IActionResult> Results()
        {
            var results = _sessionService.Get<List<StaffSearchResult>>("SearchResults");
            return View(results);
        }

        [HttpGet("select")]
        public async Task<IActionResult> Select(int stdCode, long uln)
        {
            var learner = await _apiClient.GetLearner(stdCode, uln);
            return View(learner);
        }
    }

    public class SearchViewModel
    {
        public string SearchString { get; set; }
    }
}