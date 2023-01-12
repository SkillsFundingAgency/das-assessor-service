using SFA.DAS.AssessorService.Api.Types.Models;
using System;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public interface IOppFinderSession
    {
        string SearchTerm { get; set; }

        string SectorFilters { get; set; }
        string LevelFilters { get; set; }

        int ApprovedPageIndex { get; set; }
        int ApprovedStandardsPerPage { get; set; }

        OppFinderApprovedSearchSortColumn ApprovedSortColumn { get; set; }

        string ApprovedSortDirection { get; set; }

        int InDevelopmentPageIndex { get; set; }
        int InDevelopmentStandardsPerPage { get; set; }

        OppFinderSearchSortColumn InDevelopmentSortColumn { get; set; }

        string InDevelopmentSortDirection { get; set; }

        int ProposedPageIndex { get; set; }
        int ProposedStandardsPerPage { get; set; }

        OppFinderSearchSortColumn ProposedSortColumn { get; set; }

        string ProposedSortDirection { get; set; }
    }

    public class OppFinderSession : IOppFinderSession
    {
        private readonly ISessionService _sessionService;

        public OppFinderSession(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public string SearchTerm
        {
            get
            {
                return _sessionService.Get("SearchTerm");
            }
            set
            {
                _sessionService.Set("SearchTerm", value);
            }
        }

        public string SectorFilters
        {
            get
            {
                return _sessionService.Get("SectorFilters");
            }
            set
            {
                _sessionService.Set("SectorFilters", value);
            }
        }

        public string LevelFilters
        {
            get
            {
                return _sessionService.Get("LevelFilters");
            }
            set
            {
                _sessionService.Set("LevelFilters", value);
            }
        }

        public int ApprovedPageIndex
        {
            get
            {
                return int.Parse(_sessionService.Get("ApprovedPageIndex"));
            }
            set
            {
                _sessionService.Set("ApprovedPageIndex", value);
            }
        }

        public int ApprovedStandardsPerPage
        {
            get
            {
                return int.Parse(_sessionService.Get("ApprovedStandardsPerPage"));
            }
            set
            {
                _sessionService.Set("ApprovedStandardsPerPage", value);
            }
        }

        public OppFinderApprovedSearchSortColumn ApprovedSortColumn
        {
            get
            {
                return Enum.Parse<OppFinderApprovedSearchSortColumn>(_sessionService.Get("ApprovedSortColumn"));
            }
            set
            {
                _sessionService.Set("ApprovedSortColumn", value);
            }
        }

        public string ApprovedSortDirection
        {
            get
            {
                return _sessionService.Get("ApprovedSortDirection");
            }
            set
            {
                _sessionService.Set("ApprovedSortDirection", value);
            }
        }

        public int InDevelopmentPageIndex
        {
            get
            {
                return int.Parse(_sessionService.Get("InDevelopmentPageIndex"));
            }
            set
            {
                _sessionService.Set("InDevelopmentPageIndex", value);
            }
        }

        public int InDevelopmentStandardsPerPage
        {
            get
            {
                return int.Parse(_sessionService.Get("InDevelopmentStandardsPerPage"));
            }
            set
            {
                _sessionService.Set("InDevelopmentStandardsPerPage", value);
            }
        }

        public OppFinderSearchSortColumn InDevelopmentSortColumn
        {
            get
            {
                return Enum.Parse<OppFinderSearchSortColumn>(_sessionService.Get("InDevelopmentSortColumn"));
            }
            set
            {
                _sessionService.Set("InDevelopmentSortColumn", value);
            }
        }

        public string InDevelopmentSortDirection
        {
            get
            {
                return _sessionService.Get("InDevelopmentSortDirection");
            }
            set
            {
                _sessionService.Set("InDevelopmentSortDirection", value);
            }
        }

        public int ProposedPageIndex
        {
            get
            {
                return int.Parse(_sessionService.Get("ProposedPageIndex"));
            }
            set
            {
                _sessionService.Set("ProposedPageIndex", value);
            }
        }

        public int ProposedStandardsPerPage
        {
            get
            {
                return int.Parse(_sessionService.Get("ProposedStandardsPerPage"));
            }
            set
            {
                _sessionService.Set("ProposedStandardsPerPage", value);
            }
        }

        public OppFinderSearchSortColumn ProposedSortColumn
        {
            get
            {
                return Enum.Parse<OppFinderSearchSortColumn>(_sessionService.Get("ProposedSortColumn"));
            }
            set
            {
                _sessionService.Set("ProposedSortColumn", value);
            }
        }

        public string ProposedSortDirection
        {
            get
            {
                return _sessionService.Get("ProposedSortDirection");
            }
            set
            {
                _sessionService.Set("ProposedSortDirection", value);
            }
        }
    }
}