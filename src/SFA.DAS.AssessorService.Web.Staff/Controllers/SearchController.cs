using System.Collections.Generic;
using System.Threading.Tasks;
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

        public SearchController(ILogger<SearchController> logger, ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
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
            TempData["SearchResults"] = searchResults;
            return RedirectToAction("Results");
        }

        [HttpGet("results")]
        public async Task<IActionResult> Results()
        {
            List<SearchResult> results = (List<SearchResult>)TempData["SearchResults"];
            return View(results);
        }
    }

    public class SearchViewModel
    {
        public string SearchString { get; set; }
    }
}