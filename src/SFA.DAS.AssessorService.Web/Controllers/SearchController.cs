using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Orchestrators.Search;
using SFA.DAS.AssessorService.Web.Utils;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ISearchOrchestrator _searchOrchestrator;

        public SearchController(ISearchOrchestrator searchOrchestrator)
        {
            _searchOrchestrator = searchOrchestrator;
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

            var result = await _searchOrchestrator.Search(vm);

            if (!result.SearchResults.Any()) return View("Index", vm);

            TempData.Put("Results", result);
            return RedirectToAction("Results");
        }

        [HttpGet]
        public IActionResult Results()
        {
            var vm = TempData.Get<SearchViewModel>("Results");
            if (vm == null)
            {
                return RedirectToAction("Index");
            }

            return View("Results", vm);
        }
    }
}