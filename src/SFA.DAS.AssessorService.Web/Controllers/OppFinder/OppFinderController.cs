using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.OppFinder;

namespace SFA.DAS.AssessorService.Web.Controllers.OppFinder
{
    [Route("apprenticeship-assessment-business-opportunity")]
    [CheckSession(nameof(OppFinderController), nameof(Index), nameof(IOppFinderSession.SearchTerm))]
    public class OppFinderController : Controller
    {
        private readonly IOppFinderSession _oppFinderSession;
        private readonly IOppFinderApiClient _oppFinderApiClient;

        private const int PageSetSize = 6;

        public OppFinderController(IOppFinderSession oppFinderSession, IOppFinderApiClient oppFinderApiClient)
        {
            _oppFinderSession = oppFinderSession;
            _oppFinderApiClient = oppFinderApiClient;
        }

        [HttpGet]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Ignore)]
        public async Task<IActionResult> Index()
        {
            SetDefaultSession();

            var vm = await MapViewModelFromSession();
            return View(vm);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string searchTerm)
        {
            _oppFinderSession.SearchTerm = searchTerm ?? string.Empty;
            
            var vm = await MapViewModelFromSession();
            return View("Index", vm);
        }

        [HttpGet("SearchPartial")]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> SearchPartial(string searchTerm)
        {
            // multiple partial methods should be called from the view to update each of the partial results
            _oppFinderSession.SearchTerm = searchTerm ?? string.Empty;

            var vm = await AddFilterViewModelValues(new OppFinderShowFiltersViewModel());
            return PartialView("_StandardFiltersPartial", vm);
        }

        [HttpPost(nameof(ApplyFilters))]
        public async Task<IActionResult> ApplyFilters(OppFinderApplyFiltersViewModel viewModel)
        {
            _oppFinderSession.SectorFilters = string.Join("|", viewModel.SectorFilters ?? new string[] { } );
            _oppFinderSession.LevelFilters = string.Join("|", viewModel.LevelFilters ?? new string[] { });

            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        [HttpPost(nameof(ApplyFiltersPartial))]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ApplyFiltersPartial([FromBody] OppFinderApplyFiltersViewModel viewModel)
        {
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

        [HttpGet("ChangePageApprovedStandards")]
        public async Task<IActionResult> ChangePageApprovedStandards(int pageIndex)
        {
            _oppFinderSession.ApprovedPageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View("Index", vm);
        }

        [HttpGet("ChangePageSetApprovedStandards")]
        public async Task<IActionResult> ChangePageSetApprovedStandards(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageApprovedStandards(newPageIndex);
        }

        [HttpGet("ChangePageApprovedStandardsPartial")]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ChangePageApprovedStandardsPartial(int pageIndex)
        {
            _oppFinderSession.ApprovedPageIndex = pageIndex;
            var vm = await AddApprovedViewModelValues(new OppFinderSearchViewModel());
            return PartialView("_ApprovedStandardsPartial", vm);
        }

        [HttpGet("ChangePageSetApprovedStandardsPartial")]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ChangePageSetApprovedStandardsPartial(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageApprovedStandardsPartial(newPageIndex);
        }

        [HttpGet("ShowApprovedStandardsPerPage")]
        public async Task<IActionResult> ShowApprovedStandardsPerPage(int approvedStandardsPerPage)
        {
            if (approvedStandardsPerPage > 0)
            {
                _oppFinderSession.ApprovedStandardsPerPage = approvedStandardsPerPage;
            }

            return await ChangePageApprovedStandards(1);
        }

        [HttpGet("ShowApprovedStandardsPerPagePartial")]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ShowApprovedStandardsPerPagePartial(int standardsPerPage)
        {
            if (standardsPerPage > 0)
            {
                _oppFinderSession.ApprovedStandardsPerPage = standardsPerPage;
            }

            return await ChangePageApprovedStandardsPartial(1);
        }

        [HttpGet("SortApprovedStandards")]
        public async Task<IActionResult> SortApprovedStandards(string sortColumn, string sortDirection)
        {
            UpdateApprovedSortDirection(sortColumn, sortDirection);
            return await ChangePageApprovedStandards(1);
        }

        [HttpGet("SortApprovedStandardsPartial")]
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

        [HttpGet("ChangePageInDevelopmentStandards")]
        public async Task<IActionResult> ChangePageInDevelopmentStandards(int pageIndex)
        {
            _oppFinderSession.InDevelopmentPageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View("Index", vm);
        }

        [HttpGet("ChangePageSetInDevelopmentStandards")]
        public async Task<IActionResult> ChangePageSetInDevelopmentStandards(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageInDevelopmentStandards(newPageIndex);
        }

        [HttpGet("ChangePageInDevelopmentStandardsPartial")]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ChangePageInDevelopmentStandardsPartial(int pageIndex)
        {
            _oppFinderSession.InDevelopmentPageIndex = pageIndex;
            var vm = await AddInDevelopmentViewModelValues(new OppFinderSearchViewModel());
            return PartialView("_InDevelopmentStandardsPartial", vm);
        }

        [HttpGet("ChangePageSetInDevelopmentStandardsPartial")]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ChangePageSetInDevelopmentStandardsPartial(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageInDevelopmentStandardsPartial(newPageIndex);
        }

        [HttpGet("ShowInDevelopmentStandardsPerPage")]
        public async Task<IActionResult> ShowInDevelopmentStandardsPerPage(int inDevelopmentStandardsPerPage)
        {
            if (inDevelopmentStandardsPerPage > 0)
            {
                _oppFinderSession.InDevelopmentStandardsPerPage = inDevelopmentStandardsPerPage;
            }

            return await ChangePageInDevelopmentStandards(1);
        }

        [HttpGet("ShowInDevelopmentStandardsPerPagePartial")]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ShowInDevelopmentStandardsPerPagePartial(int standardsPerPage)
        {
            if (standardsPerPage > 0)
            {
                _oppFinderSession.InDevelopmentStandardsPerPage = standardsPerPage;
            }

            return await ChangePageInDevelopmentStandardsPartial(1);
        }

        [HttpGet("SortInDevelopmentStandards")]
        public async Task<IActionResult> SortInDevelopmentStandards(string sortColumn, string sortDirection)
        {
            UpdateInDevelopmentSortDirection(sortColumn, sortDirection);
            return await ChangePageInDevelopmentStandards(1);
        }

        [HttpGet("SortInDevelopmentStandardsPartial")]
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

        [HttpGet("ChangePageProposedStandards")]
        public async Task<IActionResult> ChangePageProposedStandards(int pageIndex)
        {            
            _oppFinderSession.ProposedPageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View("Index", vm);
        }

        [HttpGet("ChangePageSetProposedStandards")]
        public async Task<IActionResult> ChangePageSetProposedStandards(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageProposedStandards(newPageIndex);
        }

        [HttpGet("ChangePageProposedStandardsPartial")]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ChangePageProposedStandardsPartial(int pageIndex)
        {
            _oppFinderSession.ProposedPageIndex = pageIndex;
            var vm = await AddProposedViewModelValues(new OppFinderSearchViewModel()); 
            return PartialView("_ProposedStandardsPartial", vm);
        }

        [HttpGet("ChangePageSetProposedStandardsPartial")]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ChangePageSetProposedStandardsPartial(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageProposedStandardsPartial(newPageIndex);
        }

        [HttpGet("ShowProposedStandardsPerPage")]
        public async Task<IActionResult> ShowProposedStandardsPerPage(int proposedStandardsPerPage)
        {
            if (proposedStandardsPerPage > 0)
            {
                _oppFinderSession.ProposedStandardsPerPage = proposedStandardsPerPage;
            }

            return await ChangePageProposedStandards(1);
        }

        [HttpGet("ShowProposedStandardsPerPagePartial")]
        [CheckSession(nameof(IOppFinderSession.SearchTerm), CheckSession.Error)]
        public async Task<IActionResult> ShowProposedStandardsPerPagePartial(int standardsPerPage)
        {
            if (standardsPerPage > 0)
            {
                _oppFinderSession.ProposedStandardsPerPage = standardsPerPage;
            }

            return await ChangePageProposedStandardsPartial(1);
        }

        [HttpGet("SortProposedStandards")]
        public async Task<IActionResult> SortProposedStandards(string sortColumn, string sortDirection)
        {
            UpdateProposedSortDirection(sortColumn, sortDirection);
            return await ChangePageProposedStandards(1);
        }

        [HttpGet("SortProposedStandardsPartial")]
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
                PageSetSize = PageSetSize
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
                PageSetSize = PageSetSize,
                NonApprovedType = "InDevelopment"
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
                PageSetSize = PageSetSize,
                NonApprovedType = "Proposed"
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

            var pageIndex = 1;
            _oppFinderSession.ApprovedPageIndex = pageIndex;
            _oppFinderSession.InDevelopmentPageIndex = pageIndex;
            _oppFinderSession.ProposedPageIndex = pageIndex;

            var standardsPerPage = 10;
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