using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardRepository
    {
        Task<string> UpsertApprovedStandards(List<StandardCollation> standards);
        Task<string> UpsertNonApprovedStandards(List<StandardNonApprovedCollation> standards);

        Task<List<StandardCollation>> GetStandardCollations();
        Task<StandardCollation> GetStandardCollationByStandardId(int standardId);
        Task<StandardCollation> GetStandardCollationByReferenceNumber(string referenceNumber);

        Task<List<StandardNonApprovedCollation>> GetStandardNonApprovedCollations();
        Task<StandardNonApprovedCollation> GetStandardNonApprovedCollationByReferenceNumber(string referenceNumber);

        Task<DateTime?> GetDateOfLastStandardCollation();
        Task<int> GetEpaoStandardsCount(string endPointAssessorOrganisationId);
        Task<EpoRegisteredStandardsResult> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId,
            int pageSize, int? pageIndex);

        Task<EpaoPipelineStandardsResult> GetEpaoPipelineStandards(string endPointAssessorOrganisationId,
            string orderBy, string orderDirection, int pageSize, int? pageIndex);
        Task<List<EpaoPipelineStandardExtract>> GetEpaoPipelineStandardsExtract(string endPointAssessorOrganisationId);

        Task<ApprovedStandardsResult> GetOppFinderApprovedStandards(string sortColumn, int sortAscending, int pageSize, int pageIndex);
        Task<NonApprovedStandardsResult> GetOppFinderNonApprovedStandards(string sortColumn, int sortAscending, int pageSize, int pageIndex, string nonApprovedType);
        
    }

    public class EpoRegisteredStandardsResult
    {
        public IEnumerable<EPORegisteredStandards> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }

    public class EpaoPipelineStandardsResult
    {
        public IEnumerable<EpaoPipelineStandard> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }

    public class ApprovedStandardsResult
    {
        public IEnumerable<OppFinderApprovedStandard> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }

    public class NonApprovedStandardsResult
    {
        public IEnumerable<OppFinderNonApprovedStandard> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }
}
