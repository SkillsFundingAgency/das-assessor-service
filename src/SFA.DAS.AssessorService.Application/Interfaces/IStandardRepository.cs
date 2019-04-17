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
        Task<string> UpsertStandards(List<StandardCollation> standards);
        Task<List<StandardCollation>> GetStandardCollations();
        Task<StandardCollation> GetStandardCollationByStandardId(int standardId);
        Task<StandardCollation> GetStandardCollationByReferenceNumber(string referenceNumber);
        Task<DateTime?> GetDateOfLastStandardCollation();
        Task<int> GetEpaoStandardsCount(string endPointAssessorOrganisationId);
        Task<EpoRegisteredStandardsResult> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId,
            int pageSize, int? pageIndex);

        Task<EpaoPipelineStandardsResult> GetEpaoPipelineStandards(string endPointAssessorOrganisationId,
            string orderBy, string orderDirection, int pageSize, int? pageIndex);
        Task<List<EpaoPipelineStandardExtract>> GetEpaoPipelineStandardsExtract(string endPointAssessorOrganisationId);
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
}
