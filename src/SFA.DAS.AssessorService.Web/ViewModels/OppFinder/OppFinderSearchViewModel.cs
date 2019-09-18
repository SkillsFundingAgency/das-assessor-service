using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.OppFinder
{
    public class OppFinderSearchViewModel
    {
        public string SearchTerm { get; set; }

        public PaginatedList<OppFinderSearchResult> ApprovedStandards { get; set; }

        public int ApprovedStandardsPerPage { get; set; }

        public OppFinderApprovedSearchSortColumn ApprovedSortColumn { get; set; }

        public string ApprovedSortDirection { get; set; }

        public int ApprovedPageIndex { get; set; }

        public PaginatedList<OppFinderSearchResult> InDevelopmentStandards { get; set; }

        public int InDevelopmentStandardsPerPage { get; set; }

        public OppFinderSearchSortColumn InDevelopmentSortColumn { get; set; }

        public string InDevelopmentSortDirection { get; set; }

        public int InDevelopmentPageIndex { get; set; }

        public PaginatedList<OppFinderSearchResult> ProposedStandards { get; set; }

        public int ProposedStandardsPerPage { get; set; }

        public OppFinderSearchSortColumn ProposedSortColumn { get; set; }

        public string ProposedSortDirection { get; set; }

        public int ProposedPageIndex { get; set; }

        public List<SelectListItem> StandardsPerPage { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "10", Text = "10" },
            new SelectListItem { Value = "50", Text = "50" },
            new SelectListItem { Value = "100", Text = "100"  },
            new SelectListItem { Value = "500", Text = "500"  }
        };
    }
}
