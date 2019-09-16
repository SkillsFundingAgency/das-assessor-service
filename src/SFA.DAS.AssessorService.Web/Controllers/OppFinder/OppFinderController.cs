using System;
using System.Collections.Generic;
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

        public async Task<IActionResult> Index()
        {
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

            var vm = await MapViewModelFromSession();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Search()
        {
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

            var vm = await MapViewModelFromSession();
            return View("Index", vm);
        }

        public async Task<IActionResult> ChangePageApprovedStandards(int pageIndex)
        {
            _oppFinderSession.ApprovedPageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View("Index", vm);
        }

        public async Task<IActionResult> ChangePageSetApprovedStandards(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageApprovedStandards(newPageIndex);
        }

        public async Task<IActionResult> ChangePageApprovedStandardsPartial(int pageIndex)
        {
            _oppFinderSession.ApprovedPageIndex = pageIndex;
            var approvedStandards = await GetPageApprovedStandards(_oppFinderSession.ApprovedPageIndex);

            var vm = new OppFinderSearchViewModel
            {
                ApprovedStandards = approvedStandards,
                ApprovedStandardsPerPage = _oppFinderSession.ApprovedStandardsPerPage,
                ApprovedSortColumn = _oppFinderSession.ApprovedSortColumn,
                ApprovedSortDirection = _oppFinderSession.ApprovedSortDirection,
                ApprovedPageIndex = _oppFinderSession.ApprovedPageIndex
            };

            return PartialView("_ApprovedStandardsPartial", vm);
        }

        public async Task<IActionResult> ChangePageSetApprovedStandardsPartial(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageApprovedStandardsPartial(newPageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ShowApprovedStandardsPerPage(int approvedStandardsPerPage)
        {
            if (approvedStandardsPerPage > 0)
            {
                _oppFinderSession.ApprovedStandardsPerPage = approvedStandardsPerPage;
            }

            return await ChangePageApprovedStandards(1);
        }

        public async Task<IActionResult> ShowApprovedStandardsPerPagePartial(int standardsPerPage)
        {
            if (standardsPerPage > 0)
            {
                _oppFinderSession.ApprovedStandardsPerPage = standardsPerPage;
            }

            return await ChangePageApprovedStandardsPartial(1);
        }

        public async Task<IActionResult> SortApprovedStandards(string sortColumn, string sortDirection)
        {
            UpdateApprovedSortDirection(sortColumn, sortDirection);
            return await ChangePageApprovedStandards(1);
        }

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

        public async Task<IActionResult> ChangePageInDevelopmentStandards(int pageIndex)
        {
            _oppFinderSession.InDevelopmentPageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View("Index", vm);
        }

        public async Task<IActionResult> ChangePageSetInDevelopmentStandards(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageInDevelopmentStandards(newPageIndex);
        }

        public async Task<IActionResult> ChangePageInDevelopmentStandardsPartial(int pageIndex)
        {
            _oppFinderSession.InDevelopmentPageIndex = pageIndex;
            var inDevelopmentStandards = await GetPageInDevelopmentStandards(_oppFinderSession.InDevelopmentPageIndex);
            
            var vm = new OppFinderSearchViewModel
            {
                InDevelopmentStandards = inDevelopmentStandards,
                InDevelopmentStandardsPerPage = _oppFinderSession.InDevelopmentStandardsPerPage,
                InDevelopmentSortColumn = _oppFinderSession.InDevelopmentSortColumn,
                InDevelopmentSortDirection = _oppFinderSession.InDevelopmentSortDirection,
                InDevelopmentPageIndex = _oppFinderSession.InDevelopmentPageIndex
            };

            return PartialView("_InDevelopmentStandardsPartial", vm);
        }

        public async Task<IActionResult> ChangePageSetInDevelopmentStandardsPartial(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageInDevelopmentStandardsPartial(newPageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ShowInDevelopmentStandardsPerPage(int inDevelopmentStandardsPerPage)
        {
            if (inDevelopmentStandardsPerPage > 0)
            {
                _oppFinderSession.InDevelopmentStandardsPerPage = inDevelopmentStandardsPerPage;
            }

            return await ChangePageInDevelopmentStandards(1);
        }

        public async Task<IActionResult> ShowInDevelopmentStandardsPerPagePartial(int standardsPerPage)
        {
            if (standardsPerPage > 0)
            {
                _oppFinderSession.InDevelopmentStandardsPerPage = standardsPerPage;
            }

            return await ChangePageInDevelopmentStandardsPartial(1);
        }

        public async Task<IActionResult> SortInDevelopmentStandards(string sortColumn, string sortDirection)
        {
            UpdateInDevelopmentSortDirection(sortColumn, sortDirection);
            return await ChangePageInDevelopmentStandards(1);
        }

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

        public async Task<IActionResult> ChangePageProposedStandards(int pageIndex)
        {            
            _oppFinderSession.ProposedPageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View("Index", vm);
        }

        public async Task<IActionResult> ChangePageSetProposedStandards(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageProposedStandards(newPageIndex);
        }

        public async Task<IActionResult> ChangePageProposedStandardsPartial(int pageIndex)
        {
            _oppFinderSession.ProposedPageIndex = pageIndex;
            var proposedStandards = await GetPageProposedStandards(_oppFinderSession.ProposedPageIndex);
            
            var vm = new OppFinderSearchViewModel
            {
                ProposedStandards = proposedStandards,
                ProposedStandardsPerPage = _oppFinderSession.ProposedStandardsPerPage,
                ProposedSortColumn = _oppFinderSession.ProposedSortColumn,
                ProposedSortDirection = _oppFinderSession.ProposedSortDirection,
                ProposedPageIndex = _oppFinderSession.ProposedPageIndex
            };

            return PartialView("_ProposedStandardsPartial", vm);
        }

        public async Task<IActionResult> ChangePageSetProposedStandardsPartial(int pageSetIndex)
        {
            var newPageIndex = PaginatedList.GetFirstPageOfPageSet(pageSetIndex, PageSetSize);
            return await ChangePageProposedStandardsPartial(newPageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ShowProposedStandardsPerPage(int proposedStandardsPerPage)
        {
            if (proposedStandardsPerPage > 0)
            {
                _oppFinderSession.ProposedStandardsPerPage = proposedStandardsPerPage;
            }

            return await ChangePageProposedStandards(1);
        }

        public async Task<IActionResult> ShowProposedStandardsPerPagePartial(int standardsPerPage)
        {
            if (standardsPerPage > 0)
            {
                _oppFinderSession.ProposedStandardsPerPage = standardsPerPage;
            }

            return await ChangePageProposedStandardsPartial(1);
        }

        public async Task<IActionResult> SortProposedStandards(string sortColumn, string sortDirection)
        {
            UpdateProposedSortDirection(sortColumn, sortDirection);
            return await ChangePageProposedStandards(1);
        }

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

        private async Task<PaginatedList<OppFinderSearchResult>> GetPageApprovedStandards(int pageIndex)
        {
            var pageSize = _oppFinderSession.ApprovedStandardsPerPage;
            var sortColumn = _oppFinderSession.ApprovedSortColumn;
            var sortAscending = _oppFinderSession.ApprovedSortDirection == "Asc" ? 1 : 0;

            var response = await  _oppFinderApiClient.GetApprovedStandards(
                sortColumn.ToString(),
                sortAscending,
                pageSize,
                pageIndex,
                PageSetSize);

            return response.Standards.Convert<OppFinderSearchResult>();
        }

        private async Task<PaginatedList<OppFinderSearchResult>> GetPageInDevelopmentStandards(int pageIndex)
        {
            var pageSize = _oppFinderSession.InDevelopmentStandardsPerPage;
            var sortColumn = _oppFinderSession.InDevelopmentSortColumn;
            var sortAscending = _oppFinderSession.InDevelopmentSortDirection == "Asc" ? 1 : 0;
            var nonApprovedType = "InDevelopment";

            var response = await _oppFinderApiClient.GetNonApprovedStandards(
                sortColumn.ToString(),
                sortAscending,
                pageSize,
                pageIndex,
                PageSetSize,
                nonApprovedType);

            return response.Standards;
        }

        private async Task<PaginatedList<OppFinderSearchResult>> GetPageProposedStandards(int pageIndex)
        {
            var pageSize = _oppFinderSession.ProposedStandardsPerPage;
            var sortColumn = _oppFinderSession.ProposedSortColumn;
            var sortAscending = _oppFinderSession.ProposedSortDirection == "Asc" ? 1 : 0;
            var nonApprovedType = "Proposed";

            var response = await _oppFinderApiClient.GetNonApprovedStandards(
                sortColumn.ToString(),
                sortAscending,
                pageSize,
                pageIndex,
                PageSetSize,
                nonApprovedType);

            return response.Standards;
        }

        private async Task<OppFinderSearchViewModel> MapViewModelFromSession()
        {
            return new OppFinderSearchViewModel
            {
                ApprovedStandards = await GetPageApprovedStandards(_oppFinderSession.ApprovedPageIndex),
                ApprovedStandardsPerPage = _oppFinderSession.ApprovedStandardsPerPage,
                ApprovedSortColumn = _oppFinderSession.ApprovedSortColumn,
                ApprovedSortDirection = _oppFinderSession.ApprovedSortDirection,
                ApprovedPageIndex = _oppFinderSession.ApprovedPageIndex,
                InDevelopmentStandards = await GetPageInDevelopmentStandards(_oppFinderSession.ProposedPageIndex),
                InDevelopmentStandardsPerPage = _oppFinderSession.InDevelopmentStandardsPerPage,
                InDevelopmentSortColumn = _oppFinderSession.InDevelopmentSortColumn,
                InDevelopmentSortDirection = _oppFinderSession.InDevelopmentSortDirection,
                InDevelopmentPageIndex = _oppFinderSession.InDevelopmentPageIndex,
                ProposedStandards = await GetPageProposedStandards(_oppFinderSession.ProposedPageIndex),
                ProposedStandardsPerPage = _oppFinderSession.ProposedStandardsPerPage,
                ProposedSortColumn = _oppFinderSession.ProposedSortColumn,
                ProposedSortDirection = _oppFinderSession.ProposedSortDirection,
                ProposedPageIndex = _oppFinderSession.ProposedPageIndex
            };
        }
    }
}