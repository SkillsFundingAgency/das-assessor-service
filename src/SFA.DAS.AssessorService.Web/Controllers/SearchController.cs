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
    [Route("[controller]/[action]")]
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
        [Route("/")]
        [Route("/[controller]")]
        public IActionResult Index()
        {
            _contextAccessor.HttpContext.Session.Remove("SearchResults");
            return View("Index");
        }

        [HttpPost]
        [Route("/")]
        [Route("/[controller]")]
        public async Task<IActionResult> Index([FromForm] SearchRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _searchOrchestrator.Search(vm);
            if (!result.SearchResults.Any()) return View("Index", vm);
            _contextAccessor.HttpContext.Session.Put("SearchResults", result);

            if (result.SearchResults.Count() > 1)
            {
                GetChooseStandardViewModel(vm);
                return RedirectToAction("ChooseStandard");
            }

            GetSelectedStandardViewModel(result);
            return RedirectToAction("Result");
        }

        private void GetChooseStandardViewModel(SearchRequestViewModel vm)
        {
            var chooseStandardViewModel = new ChooseStandardViewModel
            {
                SearchResults = vm.SearchResults
            };

            _contextAccessor.HttpContext.Session.Put("SearchResultsChooseStandard", chooseStandardViewModel);
        }

        private void GetSelectedStandardViewModel(SearchRequestViewModel result)
        {
            var selectedStandardViewModel = new SelectedStandardViewModel()
            {
                Standard = result.SearchResults.First().Standard,
                StdCode = result.SearchResults.First().StdCode,
                Uln = result.SearchResults.First().Uln,
                GivenNames = result.SearchResults.First().GivenNames,
                FamilyName = result.SearchResults.First().FamilyName,
                CertificateReference = result.SearchResults.First().CertificateReference,
                OverallGrade = result.SearchResults.First().OverallGrade
            };

            _contextAccessor.HttpContext.Session.Put("SelectedStandard", selectedStandardViewModel);
        }

        [HttpGet]
        public IActionResult Result()
        {
            var vm = _contextAccessor.HttpContext.Session.Get<SelectedStandardViewModel>("SelectedStandard");
            if (vm == null)
            {
                return RedirectToAction("Index");
            }

            return View("Result", vm);
        }

        [HttpGet(Name = "choose")]
        public IActionResult ChooseStandard()
        {
            var vm = _contextAccessor.HttpContext.Session.Get<ChooseStandardViewModel>("SearchResultsChooseStandard");
            if (vm == null)
            {
                return RedirectToAction("Index");
            }

            return View("ChooseStandard", vm);
        }

        [HttpPost(Name = "choose")]
        public IActionResult ChooseStandard(ChooseStandardViewModel chooseStandardViewModel)
        {
            var vm = _contextAccessor.HttpContext.Session.Get<SearchRequestViewModel>("SearchResults");
            if (vm == null)
            {
                return RedirectToAction("Index");
            }

            var selected = vm.SearchResults
                .Single(sr => sr.StdCode == chooseStandardViewModel.SelectedStandardCode);

            var selectedStandardViewModel = new SelectedStandardViewModel()
            {
                Standard = selected.Standard,
                StdCode = selected.StdCode,
                Uln = selected.Uln,
                GivenNames = selected.GivenNames,
                FamilyName = selected.FamilyName,
                CertificateReference = selected.CertificateReference,
                OverallGrade = selected.OverallGrade
            };

            _contextAccessor.HttpContext.Session.Put("SelectedStandard", selectedStandardViewModel);
            
            return RedirectToAction("Result");
        }
    }
}