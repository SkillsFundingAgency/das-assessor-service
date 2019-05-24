using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs
{
    public interface IAssessmentOrgsApiClient : IDisposable
    {
        Task<IEnumerable<StandardOrganisationSummary>> FindAllStandardsByOrganisationIdAsync(string organisationId);
        Task<Standard> GetStandard(int standardId);
        Task<List<Standard>> GetAllStandards();
        Task<List<StandardSummary>> GetAllStandardsV2();
        Task<List<StandardSummary>> GetAllStandardSummaries();
        Task<Provider> GetProvider(long providerUkPrn);
    }
}