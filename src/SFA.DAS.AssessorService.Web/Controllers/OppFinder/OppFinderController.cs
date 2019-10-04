using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Models;
using SFA.DAS.AssessorService.Web.ViewModels.OppFinder;

namespace SFA.DAS.AssessorService.Web.Controllers.OppFinder
{
    [Route(OppFinderRoute)]
    [CheckSession(OppFinderRoute, nameof(ResetSession), nameof(IOppFinderSession.SearchTerm))]
    [TypeFilter(typeof(OppFinderExceptionFilterAttribute))]
    public class OppFinderController : Controller
    {
        private const string OppFinderRoute = "find-an-assessment-opportunity";

        private readonly IOppFinderSession _oppFinderSession;
        private readonly IOppFinderApiClient _oppFinderApiClient;
        private readonly IValidationApiClient _validationApiClient;
        private readonly ILogger<OppFinderController> _logger;

        private const int DefaultPageSetSize = 6;
        private const int DefaultPageIndex = 1;
        private const int DefaultStandardsPerPage = 100;

        public OppFinderController(IOppFinderSession oppFinderSession, IOppFinderApiClient oppFinderApiClient, IValidationApiClient validationApiClient, ILogger<OppFinderController> logger)
        {
            _oppFinderSession = oppFinderSession;
            _oppFinderApiClient = oppFinderApiClient;
            _validationApiClient = validationApiClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        [HttpGet(nameof(ResetSession))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Ignore)]
        public IActionResult ResetSession()
        {
            SetDefaultSession();
            return RedirectToAction(string.Empty, OppFinderRoute);
        }

        [HttpGet(nameof(ContactUs))]
        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpGet(nameof(Cookies))]
        public IActionResult Cookies()
        {
            return View();
        }

        [HttpGet(nameof(Privacy))]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet(nameof(Error))]
        public IActionResult Error()
        {
            return View("ErrorOppFinder", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        [HttpGet(nameof(Search))]
        public async Task<IActionResult> Search(string searchTerm)
        {
            _oppFinderSession.SearchTerm = searchTerm ?? string.Empty;

            // reset the page indexes as the new results may have less pages
            _oppFinderSession.ApprovedPageIndex = 1;
            _oppFinderSession.InDevelopmentPageIndex = 1;
            _oppFinderSession.ProposedPageIndex = 1;

            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        [HttpGet(nameof(SearchPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> SearchPartial(string searchTerm)
        {
            // multiple partial methods should be called from the view to update each of the partial results
            // each of these will reset the page index to account for fewer results
            _oppFinderSession.SearchTerm = searchTerm ?? string.Empty;

            // the selected filter do not change but their values will change for the new search
            var vm = await AddFilterViewModelValues(new OppFinderShowFiltersViewModel());
            return PartialView("_StandardFiltersPartial", vm);
        }

        [HttpPost(nameof(ApplyFilters))]
        public async Task<IActionResult> ApplyFilters(OppFinderApplyFiltersViewModel viewModel)
        {
            _oppFinderSession.SectorFilters = string.Join("|", viewModel.SectorFilters ?? new string[] { });
            _oppFinderSession.LevelFilters = string.Join("|", viewModel.LevelFilters ?? new string[] { });

            // reset the page indexes as the new results may have less pages
            _oppFinderSession.ApprovedPageIndex = 1;
            _oppFinderSession.InDevelopmentPageIndex = 1;
            _oppFinderSession.ProposedPageIndex = 1;

            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        [HttpPost(nameof(ApplyFiltersPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ApplyFiltersPartial([FromBody] OppFinderApplyFiltersViewModel viewModel)
        {
            // multiple partial methods should be called from the view to update each of the partial results
            // each of these will reset the page index to account for fewer results
            _oppFinderSession.SectorFilters = string.Join("|", viewModel.SectorFilters ?? new string[] { });
            _oppFinderSession.LevelFilters = string.Join("|", viewModel.LevelFilters ?? new string[] { });

            var vm = await AddFilterViewModelValues(new OppFinderShowFiltersViewModel());
            return PartialView("_StandardFiltersPartial", vm);
        }

        [HttpGet(nameof(ResetFilters))]
        public async Task<IActionResult> ResetFilters()
        {
            _oppFinderSession.SectorFilters = string.Empty;
            _oppFinderSession.LevelFilters = string.Empty;

            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        [HttpGet(nameof(ResetFiltersPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ResetFiltersPartial()
        {
            _oppFinderSession.SectorFilters = string.Empty;
            _oppFinderSession.LevelFilters = string.Empty;

            var vm = await AddFilterViewModelValues(new OppFinderShowFiltersViewModel());
            return PartialView("_StandardFiltersPartial", vm);
        }

        [HttpGet(nameof(RefreshFiltersPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> RefreshFiltersPartial()
        {
            var vm = await AddFilterViewModelValues(new OppFinderShowFiltersViewModel());
            return PartialView("_StandardFiltersPartial", vm);
        }

        [HttpGet(nameof(ChangePageApprovedStandards))]
        public async Task<IActionResult> ChangePageApprovedStandards(int pageIndex = DefaultPageIndex)
        {
            _oppFinderSession.ApprovedPageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        [HttpGet(nameof(ChangePageApprovedStandardsPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ChangePageApprovedStandardsPartial(int pageIndex = DefaultPageIndex)
        {
            _oppFinderSession.ApprovedPageIndex = pageIndex;
            var vm = await AddApprovedViewModelValues(new OppFinderSearchViewModel());
            return PartialView("_ApprovedStandardsPartial", vm);
        }

        [HttpGet(nameof(ShowApprovedStandardsPerPage))]
        public async Task<IActionResult> ShowApprovedStandardsPerPage(int approvedStandardsPerPage = DefaultStandardsPerPage)
        {
            if (approvedStandardsPerPage > 0)
            {
                _oppFinderSession.ApprovedStandardsPerPage = approvedStandardsPerPage;
            }

            return await ChangePageApprovedStandards(1);
        }

        [HttpGet(nameof(ShowApprovedStandardsPerPagePartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ShowApprovedStandardsPerPagePartial(int standardsPerPage = DefaultStandardsPerPage)
        {
            if (standardsPerPage > 0)
            {
                _oppFinderSession.ApprovedStandardsPerPage = standardsPerPage;
            }

            return await ChangePageApprovedStandardsPartial(1);
        }

        [HttpGet(nameof(SortApprovedStandards))]
        public async Task<IActionResult> SortApprovedStandards(string sortColumn, string sortDirection)
        {
            UpdateApprovedSortDirection(sortColumn, sortDirection);
            return await ChangePageApprovedStandards(1);
        }

        [HttpGet(nameof(SortApprovedStandardsPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> SortApprovedStandardsPartial(string sortColumn, string sortDirection)
        {
            UpdateApprovedSortDirection(sortColumn, sortDirection);
            return await ChangePageApprovedStandardsPartial(1);
        }

        private void UpdateApprovedSortDirection(string sortColumnName, string sortDirection)
        {
            if (Enum.TryParse(sortColumnName, true, out OppFinderApprovedSearchSortColumn sortColumn))
            {
                if (_oppFinderSession.ApprovedSortColumn == sortColumn)
                {
                    _oppFinderSession.ApprovedSortDirection = sortDirection;
                }
                else
                {
                    _oppFinderSession.ApprovedSortColumn = sortColumn;
                    _oppFinderSession.ApprovedSortDirection = "Asc";
                }
            }
        }

        [HttpGet(nameof(ChangePageInDevelopmentStandards))]
        public async Task<IActionResult> ChangePageInDevelopmentStandards(int pageIndex = DefaultPageIndex)
        {
            _oppFinderSession.InDevelopmentPageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        [HttpGet(nameof(ChangePageInDevelopmentStandardsPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ChangePageInDevelopmentStandardsPartial(int pageIndex = DefaultPageIndex)
        {
            _oppFinderSession.InDevelopmentPageIndex = pageIndex;
            var vm = await AddInDevelopmentViewModelValues(new OppFinderSearchViewModel());
            return PartialView("_InDevelopmentStandardsPartial", vm);
        }

        [HttpGet(nameof(ShowInDevelopmentStandardsPerPage))]
        public async Task<IActionResult> ShowInDevelopmentStandardsPerPage(int inDevelopmentStandardsPerPage = DefaultStandardsPerPage)
        {
            if (inDevelopmentStandardsPerPage > 0)
            {
                _oppFinderSession.InDevelopmentStandardsPerPage = inDevelopmentStandardsPerPage;
            }

            return await ChangePageInDevelopmentStandards(1);
        }

        [HttpGet(nameof(ShowInDevelopmentStandardsPerPagePartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ShowInDevelopmentStandardsPerPagePartial(int standardsPerPage = DefaultStandardsPerPage)
        {
            if (standardsPerPage > 0)
            {
                _oppFinderSession.InDevelopmentStandardsPerPage = standardsPerPage;
            }

            return await ChangePageInDevelopmentStandardsPartial(1);
        }

        [HttpGet(nameof(SortInDevelopmentStandards))]
        public async Task<IActionResult> SortInDevelopmentStandards(string sortColumn, string sortDirection)
        {
            UpdateInDevelopmentSortDirection(sortColumn, sortDirection);
            return await ChangePageInDevelopmentStandards(1);
        }

        [HttpGet(nameof(SortInDevelopmentStandardsPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> SortInDevelopmentStandardsPartial(string sortColumn, string sortDirection)
        {
            UpdateInDevelopmentSortDirection(sortColumn, sortDirection);
            return await ChangePageInDevelopmentStandardsPartial(1);
        }

        private void UpdateInDevelopmentSortDirection(string sortColumnName, string sortDirection)
        {
            if (Enum.TryParse(sortColumnName, true, out OppFinderSearchSortColumn sortColumn))
            {
                if (_oppFinderSession.InDevelopmentSortColumn == sortColumn)
                {
                    _oppFinderSession.InDevelopmentSortDirection = sortDirection;
                }
                else
                {
                    _oppFinderSession.InDevelopmentSortColumn = sortColumn;
                    _oppFinderSession.InDevelopmentSortDirection = "Asc";
                }
            }
        }

        [HttpGet(nameof(ChangePageProposedStandards))]
        public async Task<IActionResult> ChangePageProposedStandards(int pageIndex = DefaultPageIndex)
        {
            _oppFinderSession.ProposedPageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        [HttpGet(nameof(ChangePageProposedStandardsPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ChangePageProposedStandardsPartial(int pageIndex = DefaultPageIndex)
        {
            _oppFinderSession.ProposedPageIndex = pageIndex;
            var vm = await AddProposedViewModelValues(new OppFinderSearchViewModel());
            return PartialView("_ProposedStandardsPartial", vm);
        }

        [HttpGet(nameof(ShowProposedStandardsPerPage))]
        public async Task<IActionResult> ShowProposedStandardsPerPage(int proposedStandardsPerPage = DefaultStandardsPerPage)
        {
            if (proposedStandardsPerPage > 0)
            {
                _oppFinderSession.ProposedStandardsPerPage = proposedStandardsPerPage;
            }

            return await ChangePageProposedStandards(1);
        }

        [HttpGet(nameof(ShowProposedStandardsPerPagePartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ShowProposedStandardsPerPagePartial(int standardsPerPage = DefaultStandardsPerPage)
        {
            if (standardsPerPage > 0)
            {
                _oppFinderSession.ProposedStandardsPerPage = standardsPerPage;
            }

            return await ChangePageProposedStandardsPartial(1);
        }

        [HttpGet(nameof(SortProposedStandards))]
        public async Task<IActionResult> SortProposedStandards(string sortColumn, string sortDirection)
        {
            UpdateProposedSortDirection(sortColumn, sortDirection);
            return await ChangePageProposedStandards(1);
        }

        [HttpGet(nameof(SortProposedStandardsPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> SortProposedStandardsPartial(string sortColumn, string sortDirection)
        {
            UpdateProposedSortDirection(sortColumn, sortDirection);
            return await ChangePageProposedStandardsPartial(1);
        }

        private void UpdateProposedSortDirection(string sortColumnName, string sortDirection)
        {
            if (Enum.TryParse(sortColumnName, true, out OppFinderSearchSortColumn sortColumn))
            {
                if (_oppFinderSession.ProposedSortColumn == sortColumn)
                {
                    _oppFinderSession.ProposedSortDirection = sortDirection;
                }
                else
                {
                    _oppFinderSession.ProposedSortColumn = sortColumn;
                    _oppFinderSession.ProposedSortDirection = "Asc";
                }
            }
        }

        [HttpGet(nameof(ShowApprovedStandardDetails))]
        public async Task<IActionResult> ShowApprovedStandardDetails(int standardCode)
        {
            var standardDetails = await _oppFinderApiClient.
                GetApprovedStandardDetails(new GetOppFinderApprovedStandardDetailsRequest { StandardCode = standardCode });

            if (standardDetails == null)
                return RedirectToAction(string.Empty, OppFinderRoute);

            var vm = new OppFinderApprovedDetailsViewModel
            {
                PageIndex = _oppFinderSession.ApprovedPageIndex,
                StandardCode = standardCode,
                Title = standardDetails.Title,
                OverviewOfRole = standardDetails.OverviewOfRole,
                StandardLevel = standardDetails.StandardLevel,
                StandardReference = standardDetails.StandardReference,
                TotalActiveApprentices = standardDetails.TotalActiveApprentices,
                TotalCompletedAssessments = standardDetails.TotalCompletedAssessments,
                Sector = standardDetails.Sector,
                TypicalDuration = standardDetails.TypicalDuration,
                ApprovedForDelivery = standardDetails.ApprovedForDelivery,
                MaxFunding = standardDetails.MaxFunding,
                Trailblazer = standardDetails.Trailblazer?.Trim().Split(" ") ?? new string[] { },
                StandardPageUrl = standardDetails.StandardPageUrl,
                EqaProvider = standardDetails.EqaProvider,
                EqaProviderLink = standardDetails.EqaProviderLink,
                RegionResults = standardDetails.RegionResults
            };

            return View("ApprovedDetails", vm);
        }

        [HttpGet(nameof(ShowInDevelopmentStandardDetails))]
        public async Task<IActionResult> ShowInDevelopmentStandardDetails(string standardReference)
        {
            return await ShowNonApprovedStandardDetails(standardReference, StandardStatus.InDevelopment, _oppFinderSession.InDevelopmentPageIndex);
        }

        [HttpGet(nameof(ShowProposedStandardDetails))]
        public async Task<IActionResult> ShowProposedStandardDetails(string standardReference)
        {
            return await ShowNonApprovedStandardDetails(standardReference, StandardStatus.Proposed, _oppFinderSession.ProposedPageIndex);
        }

        /*
        
        THE EXPRESION OF INTEREST HAS BEEN TEMPORARILY REMOVED DUE TO BUSINESS ISSUES THIS CODE IS COMMENTED OUT TO PREVENT PUBLIC ACCESS
         
        [HttpGet(nameof(ExpressionOfInterest))]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Ignore)]
        public async Task<IActionResult> ExpressionOfInterest(string standardReference, StandardStatus? standardStatus = null, bool backLink = false)
        {
            var nonApprovedStandardDetails = await _oppFinderApiClient.
                GetNonApprovedStandardDetails(new GetOppFinderNonApprovedStandardDetailsRequest { StandardReference = standardReference });

            var approvedStandardDetails = await _oppFinderApiClient.
                GetApprovedStandardDetails(new GetOppFinderApprovedStandardDetailsRequest { StandardCode = null, StandardReference = standardReference });

            if (approvedStandardDetails != null)
            {
                var viewModel = new OppFinderExpressionOfInterestViewModel
                {
                    BackLink = backLink,
                    StandardStatus = standardStatus ?? StandardStatus.Approved,
                    StandardName = approvedStandardDetails.Title,
                    StandardLevel = approvedStandardDetails.StandardLevel,
                    StandardCode = approvedStandardDetails.StandardCode,
                    StandardReference = approvedStandardDetails.StandardReference,
                    StandardSector = approvedStandardDetails.Sector
                };

                return View(nameof(ExpressionOfInterest), viewModel);
            }
            else if(nonApprovedStandardDetails != null)
            {
                var viewModel = new OppFinderExpressionOfInterestViewModel
                {
                    BackLink = backLink,
                    StandardStatus = standardStatus ?? StandardStatus.NonApproved,
                    StandardName = nonApprovedStandardDetails.Title,
                    StandardLevel = nonApprovedStandardDetails.StandardLevel,
                    StandardReference = nonApprovedStandardDetails.StandardReference,
                    StandardSector = nonApprovedStandardDetails.Sector
                };

                return View(nameof(ExpressionOfInterest), viewModel);
            }

            // an expression of interest was made for a non-existent standard reference
            return RedirectToAction(string.Empty, OppFinderRoute);
        }

        [HttpPost(nameof(ExpressionOfInterest))]
        [ModelStatePersist(ModelStatePersist.Store)]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Ignore)]
        public async Task<IActionResult> ExpressionOfInterest(OppFinderExpressionOfInterestViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // only check if an email address has been entered - model has required validator
                    if (await _validationApiClient.ValidateEmailAddress(viewModel.Email) == false)
                    {
                        ModelState.AddModelError(nameof(OppFinderExpressionOfInterestViewModel.Email), "Enter a valid email address");
                    }

                    // only check if a phone number has been entered - as the value is optional
                    if (!string.IsNullOrEmpty(viewModel.ContactNumber) && await _validationApiClient.ValidatePhoneNumber(viewModel.ContactNumber) == false)
                    {
                        ModelState.AddModelError(nameof(OppFinderExpressionOfInterestViewModel.ContactNumber), "Enter a valid phone number");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return RedirectToAction(nameof(ExpressionOfInterest), new { standardReference = viewModel.StandardReference });
                }

                var request = new OppFinderExpressionOfInterestRequest
                {
                    StandardReference = viewModel.StandardReference,
                    Email = viewModel.Email,
                    OrganisationName = viewModel.OrganisationName,
                    ContactName = viewModel.ContactName,
                    ContactNumber = viewModel.ContactNumber
                };

                var success = await _oppFinderApiClient.RecordExpresionOfInterest(request);
                if (!success)
                {
                    throw new Exception("Unable to send an expression of interest");
                }

                var confirmViewModel = new OppFinderExpressionOfInterestConfirmViewModel
                {
                    StandardStatus = viewModel.StandardStatus,
                    StandardName = viewModel.StandardName,
                    StandardReference = viewModel.StandardReference,
                    StandardLevel = viewModel.StandardLevel,
                    StandardSector = viewModel.StandardSector
                };

                return View("ExpressionOfInterestConfirm", confirmViewModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to express interest {viewModel.StandardName}, {viewModel.Email}, {viewModel.OrganisationName}, {viewModel.ContactName}, {viewModel.ContactNumber}");
                ModelState.AddModelError(nameof(OppFinderExpressionOfInterestViewModel.StandardReference), "Unable to express interest at this time");
                return RedirectToAction(nameof(ExpressionOfInterest), new { standardReference = viewModel.StandardReference });
            }
        }
        */

        private async Task<IActionResult> ShowNonApprovedStandardDetails(string standardReference, StandardStatus standardStatus, int pageIndex)
        {
            var standardDetails = await _oppFinderApiClient.
                GetNonApprovedStandardDetails(new GetOppFinderNonApprovedStandardDetailsRequest { StandardReference = standardReference });

            if (standardDetails == null)
                return RedirectToAction(string.Empty, OppFinderRoute,
                    standardStatus == StandardStatus.InDevelopment
                        ? "in-development"
                        : "proposed");
                      
            var vm = new OppFinderNonApprovedDetailsViewModel
            {
                StandardStatus = standardStatus,
                PageIndex = pageIndex,
                Title = standardDetails.Title,
                OverviewOfRole = standardDetails.OverviewOfRole,
                StandardLevel = standardDetails.StandardLevel,
                StandardReference = standardDetails.StandardReference,
                Sector = standardDetails.Sector,
                TypicalDuration = standardDetails.TypicalDuration,
                Trailblazer = standardDetails.Trailblazer?.Trim().Split(" ") ?? new string[] { },
                StandardPageUrl = standardDetails.StandardPageUrl
            };

            return View("NonApprovedDetails", vm);
        }

        private async Task<GetOppFinderFilterStandardsResponse> GetFilterValues()
        {
            var filterStandardsRequest = new GetOppFinderFilterStandardsRequest
            {
                SearchTerm = _oppFinderSession.SearchTerm,
                SectorFilters = _oppFinderSession.SectorFilters,
                LevelFilters = _oppFinderSession.LevelFilters,
            };

            var response = await _oppFinderApiClient.GetFilterStandards(filterStandardsRequest);
            return response;
        }

        private async Task<PaginatedList<OppFinderSearchResult>> GetPageApprovedStandards(string searchTerm, int pageIndex)
        {
            var approvedStandardsRequest = new GetOppFinderApprovedStandardsRequest
            {
                SearchTerm = searchTerm,
                SectorFilters = _oppFinderSession.SectorFilters,
                LevelFilters = _oppFinderSession.LevelFilters,
                SortColumn = _oppFinderSession.ApprovedSortColumn.ToString(),
                SortAscending = _oppFinderSession.ApprovedSortDirection == "Asc" ? 1 : 0,
                PageSize = _oppFinderSession.ApprovedStandardsPerPage,
                PageIndex = pageIndex,
                PageSetSize = DefaultPageSetSize
            };

            var response = await _oppFinderApiClient.GetApprovedStandards(approvedStandardsRequest);
            return response.Standards.Convert<OppFinderSearchResult>();
        }

        private async Task<PaginatedList<OppFinderSearchResult>> GetPageInDevelopmentStandards(string searchTerm, int pageIndex)
        {
            var nonApprovedStandardsRequest = new GetOppFinderNonApprovedStandardsRequest
            {
                SearchTerm = searchTerm,
                SectorFilters = _oppFinderSession.SectorFilters,
                LevelFilters = _oppFinderSession.LevelFilters,
                SortColumn = _oppFinderSession.InDevelopmentSortColumn.ToString(),
                SortAscending = _oppFinderSession.InDevelopmentSortDirection == "Asc" ? 1 : 0,
                PageSize = _oppFinderSession.InDevelopmentStandardsPerPage,
                PageIndex = pageIndex,
                PageSetSize = DefaultPageSetSize,
                NonApprovedType = StandardStatus.InDevelopment.ToString()
            };

            var response = await _oppFinderApiClient.GetNonApprovedStandards(nonApprovedStandardsRequest);
            return response.Standards;
        }

        private async Task<PaginatedList<OppFinderSearchResult>> GetPageProposedStandards(string searchTerm, int pageIndex)
        {
            var nonApprovedStandardsRequest = new GetOppFinderNonApprovedStandardsRequest
            {
                SearchTerm = searchTerm,
                SectorFilters = _oppFinderSession.SectorFilters,
                LevelFilters = _oppFinderSession.LevelFilters,
                SortColumn = _oppFinderSession.ProposedSortColumn.ToString(),
                SortAscending = _oppFinderSession.ProposedSortDirection == "Asc" ? 1 : 0,
                PageSize = _oppFinderSession.ProposedStandardsPerPage,
                PageIndex = pageIndex,
                PageSetSize = DefaultPageSetSize,
                NonApprovedType = StandardStatus.Proposed.ToString()
            };

            var response = await _oppFinderApiClient.GetNonApprovedStandards(nonApprovedStandardsRequest);
            return response.Standards;
        }

        private async Task<OppFinderSearchViewModel> MapViewModelFromSession()
        {
            var viewModel = new OppFinderSearchViewModel();

            viewModel = await AddApprovedViewModelValues(viewModel);
            viewModel = await AddInDevelopmentViewModelValues(viewModel);
            viewModel = await AddProposedViewModelValues(viewModel);
            viewModel.ShowFiltersViewModel = await AddFilterViewModelValues(new OppFinderShowFiltersViewModel());

            return viewModel;
        }

        private async Task<OppFinderSearchViewModel> AddApprovedViewModelValues(OppFinderSearchViewModel viewModel)
        {
            viewModel.SearchTerm = _oppFinderSession.SearchTerm;
            viewModel.ApprovedStandardsPerPage = _oppFinderSession.ApprovedStandardsPerPage;
            viewModel.ApprovedSortColumn = _oppFinderSession.ApprovedSortColumn;
            viewModel.ApprovedSortDirection = _oppFinderSession.ApprovedSortDirection;
            viewModel.ApprovedPageIndex = _oppFinderSession.ApprovedPageIndex;
            viewModel.ApprovedStandards = await GetPageApprovedStandards(viewModel.SearchTerm, viewModel.ApprovedPageIndex);

            return viewModel;
        }

        private async Task<OppFinderSearchViewModel> AddInDevelopmentViewModelValues(OppFinderSearchViewModel viewModel)
        {
            viewModel.SearchTerm = _oppFinderSession.SearchTerm;
            viewModel.InDevelopmentStandardsPerPage = _oppFinderSession.InDevelopmentStandardsPerPage;
            viewModel.InDevelopmentSortColumn = _oppFinderSession.InDevelopmentSortColumn;
            viewModel.InDevelopmentSortDirection = _oppFinderSession.InDevelopmentSortDirection;
            viewModel.InDevelopmentPageIndex = _oppFinderSession.InDevelopmentPageIndex;
            viewModel.InDevelopmentStandards = await GetPageInDevelopmentStandards(viewModel.SearchTerm, viewModel.InDevelopmentPageIndex);

            return viewModel;
        }

        private async Task<OppFinderSearchViewModel> AddProposedViewModelValues(OppFinderSearchViewModel viewModel)
        {
            viewModel.SearchTerm = _oppFinderSession.SearchTerm;
            viewModel.ProposedStandardsPerPage = _oppFinderSession.ProposedStandardsPerPage;
            viewModel.ProposedSortColumn = _oppFinderSession.ProposedSortColumn;
            viewModel.ProposedSortDirection = _oppFinderSession.ProposedSortDirection;
            viewModel.ProposedPageIndex = _oppFinderSession.ProposedPageIndex;
            viewModel.ProposedStandards = await GetPageProposedStandards(viewModel.SearchTerm, viewModel.ProposedPageIndex);

            return viewModel;
        }

        private async Task<OppFinderShowFiltersViewModel> AddFilterViewModelValues(OppFinderShowFiltersViewModel viewModel)
        {
            var filterValues = await GetFilterValues();

            viewModel.SectorFilters = filterValues.SectorFilterResults;
            viewModel.SectorFilters.ForEach(p => p.Selected = _oppFinderSession.SectorFilters.Split('|').Contains(p.Category));

            viewModel.LevelFilters = filterValues.LevelFilterResults;
            viewModel.LevelFilters.ForEach(p => p.Selected = _oppFinderSession.LevelFilters.Split('|').Contains(p.Category));

            return viewModel;
        }

        private void SetDefaultSession()
        {
            _oppFinderSession.SearchTerm = string.Empty;
            _oppFinderSession.SectorFilters = string.Empty;
            _oppFinderSession.LevelFilters = string.Empty;

            var pageIndex = DefaultPageIndex;
            _oppFinderSession.ApprovedPageIndex = pageIndex;
            _oppFinderSession.InDevelopmentPageIndex = pageIndex;
            _oppFinderSession.ProposedPageIndex = pageIndex;

            var standardsPerPage = DefaultStandardsPerPage;
            _oppFinderSession.ApprovedStandardsPerPage = standardsPerPage;
            _oppFinderSession.InDevelopmentStandardsPerPage = standardsPerPage;
            _oppFinderSession.ProposedStandardsPerPage = standardsPerPage;

            var sortDirection = "Asc";
            _oppFinderSession.ApprovedSortColumn = OppFinderApprovedSearchSortColumn.StandardName;
            _oppFinderSession.ApprovedSortDirection = sortDirection;
            _oppFinderSession.InDevelopmentSortColumn = OppFinderSearchSortColumn.StandardName;
            _oppFinderSession.InDevelopmentSortDirection = sortDirection;
            _oppFinderSession.ProposedSortColumn = OppFinderSearchSortColumn.StandardName;
            _oppFinderSession.ProposedSortDirection = sortDirection;
        }
    }
}