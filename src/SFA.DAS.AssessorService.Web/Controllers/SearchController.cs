using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Search;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    [Route("[controller]/[action]")]
    public class SearchController : Controller
    {
        private readonly ISearchOrchestrator _searchOrchestrator;
        private readonly ISessionService _sessionService;

        public SearchController(ISearchOrchestrator searchOrchestrator, ISessionService sessionService)
        {
            _searchOrchestrator = searchOrchestrator;
            _sessionService = sessionService;
        }

        [HttpGet]
        [Route("/[controller]/")]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.RecordAGrade })]
        public IActionResult Index()
        {
            _sessionService.Remove("SearchResults");
            _sessionService.Remove("SelectedStandard");
            _sessionService.Remove("SearchResultsChooseStandard");
            _sessionService.Remove("EndPointAsessorOrganisationId");
            return View("Index");
        }

        [HttpPost]
        [Route("/[controller]/")]
        public async Task<IActionResult> Index([FromForm] SearchRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _searchOrchestrator.Search(vm);

            if (result.IsPrivatelyFunded)
            {
                //When there is no certficate found
                if (!result.SearchResults.Any())
                {
                    return RedirectToAction("Index", "CertificatePrivateDeclaration", vm);
                }
                //When certificate found but ULN already being used and a different familyname used
                if (result.SearchResults.Any(x => x.UlnAlreadyExists && x.IsPrivatelyFunded))
                {
                    GetSelectedStandardViewModel(result);
                    return RedirectToAction("Result");
                }
                //Certificate found but was for a uln assocaited with a user in another org
                if (result.SearchResults.Any(x => x.Uln == "0" && x.GivenNames == null && x.IsPrivatelyFunded))
                {
                    vm.SearchResults = new List<ResultViewModel>();
                    return View("Index", vm);
                }
                //Certifcate found but no standard set or a certificate reference not set, this could be a draft certificate
                if (result.SearchResults.Any(x => x.Standard == null || x.CertificateReference == null))
                {
                    return RedirectToAction("Index", "CertificatePrivateDeclaration", vm);
                }
                
            }

            if (!result.SearchResults.Any())
                return View("Index", vm);

          
            _sessionService.Set("SearchResults", result);
            
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

            _sessionService.Set("SearchResultsChooseStandard", chooseStandardViewModel);
        }

        private void GetSelectedStandardViewModel(SearchRequestViewModel result)
        {
            var resultViewModel = result.SearchResults.First();
            var selectedStandardViewModel = new SelectedStandardViewModel()
            {
                Standard = resultViewModel.Standard,
                StdCode = resultViewModel.StdCode,
                Uln = resultViewModel.Uln,
                GivenNames = resultViewModel.GivenNames,
                FamilyName = resultViewModel.FamilyName,
                CertificateReference = resultViewModel.CertificateReference,
                OverallGrade = resultViewModel.OverallGrade,
                Level = resultViewModel.Level,
                SubmittedAt = GetSubmittedAtString(resultViewModel.SubmittedAt),
                SubmittedBy = resultViewModel.SubmittedBy,
                LearnerStartDate = resultViewModel.LearnStartDate.GetValueOrDefault().ToString("d MMMM yyyy"),
                AchievementDate = resultViewModel.AchDate.GetValueOrDefault().ToString("d MMMM yyyy"),
                ShowExtraInfo = resultViewModel.ShowExtraInfo,
                UlnAlreadyExists = resultViewModel.UlnAlreadyExists,
                IsNoMatchingFamilyName = resultViewModel.IsNoMatchingFamilyName

            };

            _sessionService.Set("SelectedStandard", selectedStandardViewModel);
        }

        private string GetSubmittedAtString(DateTime? submittedAt)
        {
            return !submittedAt.HasValue ? "" : submittedAt.Value.ToString("d MMMM yyyy");
        }

        [HttpGet]
        public IActionResult Result()
        {
            var vm = _sessionService.Get<SelectedStandardViewModel>("SelectedStandard");
            if (vm == null)
            {
                return RedirectToAction("Index");
            }

            return View("Result", vm);
        }

        [HttpGet(Name = "choose")]
        public IActionResult ChooseStandard()
        {
            var vm = _sessionService.Get<ChooseStandardViewModel>("SearchResultsChooseStandard");
            if (vm == null)
            {
                return RedirectToAction("Index");
            }

            return View("ChooseStandard", vm);
        }

        [HttpPost(Name = "choose")]
        public IActionResult ChooseStandard(ChooseStandardViewModel chooseStandardViewModel)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = _sessionService.Get<ChooseStandardViewModel>("SearchResultsChooseStandard");
                return View("ChooseStandard", viewModel);
            }

            var vm = _sessionService.Get<SearchRequestViewModel>("SearchResults");
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
                OverallGrade = selected.OverallGrade,
                Level = selected.Level,
                SubmittedAt = GetSubmittedAtString(selected.SubmittedAt),
                SubmittedBy = selected.SubmittedBy,
                LearnerStartDate = selected.LearnStartDate.GetValueOrDefault().ToString("d MMMM yyyy"),
                AchievementDate = selected.AchDate.GetValueOrDefault().ToString("d MMMM yyyy"),
                ShowExtraInfo = selected.ShowExtraInfo
            };

            _sessionService.Set("SelectedStandard", selectedStandardViewModel);
            
            return RedirectToAction("Result");
        }
    }
}