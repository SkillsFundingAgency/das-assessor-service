using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;
using SFA.DAS.AssessorService.ExternalApis.StandardCollationApiClient.Types;

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
        Task<int> GetEpaoPipelineCount(string endPointAssessorOrganisationId);
        Task<EpoRegisteredStandardsResult> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId,
            int pageSize, int? pageIndex);

        Task<EpaoPipelineStandardsResult> GetEpaoPipelineStandards(string endPointAssessorOrganisationId,
            string orderBy, string orderDirection, int pageSize, int? pageIndex);
    }
}
