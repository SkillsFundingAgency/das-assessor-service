using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
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
        
        [HttpGet("results")]
        public async Task<IActionResult> Results(string searchString, int page = 1)
        {
            var searchResults = await _apiClient.Search(searchString, page);
            var searchViewModel = new SearchViewModel
            {
                PaginatedList = searchResults,
                SearchString = searchString,
                Page = page
            };

            return View(searchViewModel);
        }

        [HttpGet("select")]
        public async Task<IActionResult> Select(int stdCode, long uln, string searchString, int page = 1)
        {
            var learner = await _apiClient.GetLearner(stdCode, uln);

            var vm = new LearnerDetailViewModel
            {
                Learner = learner,
                SearchString = searchString,
                Page = page
            };

            return View(vm);
        }
    }

    public class LearnerDetailViewModel
    {
        public LearnerDetail Learner { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
    }

    public class SearchViewModel
    {
        public string SearchString { get; set; }
        public int Page { get; set; }
        public PaginatedList<StaffSearchResult> PaginatedList { get; set; }
    }
}