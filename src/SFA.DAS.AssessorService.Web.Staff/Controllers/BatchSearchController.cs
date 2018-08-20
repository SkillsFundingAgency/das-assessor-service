using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize]
    public class BatchSearchController : Controller
    {
        private readonly ILogger<BatchSearchController> _logger;
        private readonly ApiClient _apiClient;
        private readonly ISessionService _sessionService;

        public BatchSearchController(ILogger<BatchSearchController> logger, ApiClient apiClient, ISessionService sessionService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Results(int batchNumber, int page = 1)
        {
            var searchResults = await _apiClient.BatchSearch(batchNumber, page);
            var batchSearchViewModel = new BatchSearchViewModel
            {
                PaginatedList = searchResults,
                BatchNumber = batchNumber,
                Page = page
            };

            return View(batchSearchViewModel);
        }
    }

    public class BatchSearchViewModel
    {
        public int BatchNumber { get; set; }
        public int Page { get; set; }
        public PaginatedList<StaffBatchSearchResult> PaginatedList { get; set; }
    }
}