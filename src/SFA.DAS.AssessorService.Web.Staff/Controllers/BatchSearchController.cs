using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Models;
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
        public async Task<IActionResult> Index(int page = 1)
        {
            var searchResults = await _apiClient.BatchLog(page);

            var batchLogViewModel = new BatchLogViewModel
            {
                PaginatedList = searchResults,
                Page = page
            };

            return View(batchLogViewModel);
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
}