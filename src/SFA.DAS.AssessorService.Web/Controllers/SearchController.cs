﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Search;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [PrivilegeAuthorize(Privileges.RecordGrades)]
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

            if (!result.SearchResults.Any())
                return View("Index", vm);

            _sessionService.Set("SearchResults", result);
            
            if (result.SearchResults.Any(r => r.IsPrivatelyFunded && r.CertificateStatus == CertificateStatus.Draft))
                return RedirectToAction("PrivatelyFundedDraft");
            else if (result.SearchResults.Count() > 1)
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
                SearchResults = vm.SearchResults.OrderByDescending(s => s.StandardReferenceNumber)
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
                Version = resultViewModel.Version,
                VersionConfirmed = resultViewModel.VersionConfirmed,
                SubmittedAt = GetSubmittedAtString(resultViewModel.SubmittedAt),
                SubmittedBy = resultViewModel.SubmittedBy,
                LearnerStartDate = resultViewModel.LearnStartDate.GetValueOrDefault().ToString("d MMMM yyyy"),
                AchievementDate = resultViewModel.AchDate.GetValueOrDefault().ToString("d MMMM yyyy"),
                ShowExtraInfo = resultViewModel.ShowExtraInfo,
                UlnAlreadyExists = resultViewModel.UlnAlreadyExists,
                IsNoMatchingFamilyName = resultViewModel.IsNoMatchingFamilyName,
                Versions = resultViewModel.Versions
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

            if (vm != null)
            {
                return View("Result", vm);
            }
            else
            {
                var chooseStandardViewModel = _sessionService.Get<ChooseStandardViewModel>("SearchResultsChooseStandard");
                if (chooseStandardViewModel != null)
                {
                    return View("ChooseStandard", chooseStandardViewModel);
                }
            }

            return RedirectToAction("Index");
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

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.RecordAGrade })]
        public IActionResult PrivatelyFundedDraft()
        {
            return View("PrivatelyFundedDraft");
        }
    }
}