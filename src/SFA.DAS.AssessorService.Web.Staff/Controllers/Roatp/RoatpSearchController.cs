namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Roatp
{
    using System.Threading.Tasks;
    using Api.Types.Models.Roatp;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Web.Staff.Resources;
    using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

    [Authorize]
    public class RoatpSearchController : Controller
    {
        private ILogger<RoatpSearchController> _logger;
        private IRoatpApiClient _apiClient;
        private IRoatpSessionService _sessionService;
        
        public RoatpSearchController(ILogger<RoatpSearchController> logger, IRoatpApiClient apiClient,
                                     IRoatpSessionService sessionService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _sessionService = sessionService;
        }

        [HttpPost]
        public async Task<IActionResult> Search(OrganisationSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/Index.cshtml", model);
            }
            
            OrganisationSearchResults searchResults = await _apiClient.Search(model.SearchTerm);
            var viewModel = new OrganisationSearchResultsViewModel
            {
                SearchTerm = model.SearchTerm,
                Title = BuildSearchResultsTitle(searchResults.TotalCount, model.SearchTerm),
                SearchResults = searchResults.SearchResults,
                TotalCount = searchResults.TotalCount
            };
            _sessionService.SetSearchResults(viewModel);
            var actionName = "SearchResults";
            if (searchResults.TotalCount == 0)
            {
                actionName = "NoSearchResults";
            }
            return RedirectToAction(actionName);
        }

        [Route("results-found")]
        public async Task<IActionResult> SearchResults()
        {
            var model = _sessionService.GetSearchResults();

            return View("~/Views/Roatp/SearchResults.cshtml", model);
        }

        [Route("no-results-found")]
        public async Task<IActionResult> NoSearchResults()
        {
            var model = _sessionService.GetSearchResults();

            return View("~/Views/Roatp/SearchResults.cshtml", model);
        }

        [Route("refine-search")]
        public async Task<IActionResult> RefineSearch(string searchTerm)
        {
            _sessionService.SetSearchTerm(searchTerm);

            return RedirectToAction("Index", "RoatpHome");
        }

        private string BuildSearchResultsTitle(int totalCount, string searchTerm)
        {
            string title = "";
            if (totalCount == 0)
            {
                title = string.Format(RoatpSearchValidation.NoSearchResultsFound, searchTerm);
            }
            else
            {
                var resultText = "results";
                if (totalCount == 1)
                {
                    resultText = "result";
                }
                title = string.Format(RoatpSearchValidation.SearchResultsFound, totalCount, resultText, searchTerm);
            }

            return title;
        }
    }
}
