using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IOppFinderRepository
    {
        Task<bool> CreateExpressionOfInterest(string standardReference, string email, string organisationName, string contactName, string contactNumber);

        Task<OppFinderFilterStandardsResult> GetOppFinderFilterStandards(string searchTerm, string sectorFilters, string levelFilters);
        Task<OppFinderApprovedStandardsResult> GetOppFinderApprovedStandards(string searchTerm, string sectorFilters, string levelFilters, string sortColumn, int sortAscending, int pageSize, int pageIndex);
        Task<OppFinderApprovedStandardDetailsResult> GetOppFinderApprovedStandardDetails(string standardReference);
        Task<OppFinderNonApprovedStandardsResult> GetOppFinderNonApprovedStandards(string searchTerm, string sectorFilters, string levelFilters, string sortColumn, int sortAscending, int pageSize, int pageIndex, string nonApprovedType);
        Task<OppFinderNonApprovedStandardDetailsResult> GetOppFinderNonApprovedStandardDetails(string standardReference);
        Task UpdateStandardSummary();       
    }

    public class OppFinderFilterStandardsResult
    {
        public IEnumerable<OppFinderSectorFilterResult> MatchingSectorFilterResults { get; set; }
        public IEnumerable<OppFinderLevelFilterResult> MatchingLevelFilterResults { get; set; }
    }

    public class OppFinderApprovedStandardsResult
    {
        public IEnumerable<OppFinderApprovedStandard> PageOfResults { get; set; }
        public int TotalCount { get; set; }
    }

    public class OppFinderNonApprovedStandardsResult
    {
        public IEnumerable<OppFinderNonApprovedStandard> PageOfResults { get; set; }
        public int TotalCount { get; set; }
    }

    public class OppFinderApprovedStandardDetailsResult
    {
        public OppFinderApprovedStandardOverviewResult OverviewResult { get; set; }
        public List<OppFinderApprovedStandardRegionResult> RegionResults { get; set; }
        public List<OppFinderApprovedStandardVersionResult> VersionResults { get; set; }
    }

    public class OppFinderApprovedStandardOverviewResult
    {
        public int StandardCode { get; set; }
        public string StandardName { get; set; }
        public string OverviewOfRole { get; set; }
        public int StandardLevel { get; set; }
        public string StandardReference { get; set; }
        public int TotalActiveApprentices { get; set; }
        public int TotalCompletedAssessments { get; set; }
        public string Sector { get; set; }
        public int TypicalDuration { get; set; }
        public DateTime ApprovedForDelivery { get; set; }
        public string MaxFunding { get; set; }
        public string Trailblazer { get; set; }
        public string StandardPageUrl { get; set; }
        public string EqaProviderName { get; set; }
        public string EqaProviderContactName { get; set; }
        public string EqaProviderContactEmail { get; set; }
    }

    public class OppFinderNonApprovedStandardDetailsResult
    {
        public string Title { get; set; }
        public string Status { get; set; }
        public string OverviewOfRole { get; set; }
        public string Level { get; set; }
        public string IFateReferenceNumber { get; set; }
        public string Route { get; set; }
        public int TypicalDuration { get; set; }
        public string TrailblazerContact { get; set; }
        public string StandardPageUrl { get; set; }
    }

    public class OppFinderApprovedStandardRegionResult
    {
        public string Region { get; set; }
        public string EndPointAssessorsNames { get; set; }
        public int EndPointAssessors { get; set; }
        public int ActiveApprentices { get; set; }
        public int CompletedAssessments { get; set; }
    }

    public class OppFinderApprovedStandardVersionResult
    {
        public string Version { get; set; }
        public int EndPointAssessors { get; set; }
        public int ActiveApprentices { get; set; }
        public int CompletedAssessments { get; set; }
    }
}
