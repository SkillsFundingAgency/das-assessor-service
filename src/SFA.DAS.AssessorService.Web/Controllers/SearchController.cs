using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Search;
using SFA.DAS.AssessorService.Web.Utils;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class SearchController : Controller
    {
        private readonly ISearchOrchestrator _searchOrchestrator;
        private readonly IHttpContextAccessor _contextAccessor;

        public SearchController(ISearchOrchestrator searchOrchestrator, IHttpContextAccessor contextAccessor)
        {
            _searchOrchestrator = searchOrchestrator;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _contextAccessor.HttpContext.Session.Remove("SearchResults");
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

            _contextAccessor.HttpContext.Session.Put("SearchResults", result);
            //TempData.Put("Results", result);
            return RedirectToAction("Results");
        }

        [HttpGet]
        public IActionResult Results()
        {
            var vm = _contextAccessor.HttpContext.Session.Get<SearchViewModel>("SearchResults");
            //var vm = TempData.Get<SearchViewModel>("Results");
            if (vm == null)
            {
                return RedirectToAction("Index");
            }

            return View("Results", vm);
        }
    }
}