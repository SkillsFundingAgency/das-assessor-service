using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs
{
    public class AssessmentOrgsApiClient : ApiClientBase, IAssessmentOrgsApiClient
    {
        public AssessmentOrgsApiClient(string baseUri = null) : base(baseUri)
        {
        }

        public AssessmentOrgsApiClient(HttpClient httpClient) : base(httpClient)
        {
        }
        
        public async Task<IEnumerable<StandardOrganisationSummary>> FindAllStandardsByOrganisationIdAsync(string organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/assessment-organisations/{organisationId}/standards"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardOrganisationSummary>>(request);
            }
        }

        public async Task<Standard> GetStandard(int standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/standards/{standardId}"))
            {
                return await RequestAndDeserialiseAsync<Standard>(request);
            }
        }

        public async Task<List<Standard>> GetAllStandards()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/standards"))
            {
                return await RequestAndDeserialiseAsync<List<Standard>>(request);
            }
        }

        public async Task<List<StandardSummary>> GetAllStandardsV2()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/standards/v2"))
            {
                return await RequestAndDeserialiseAsync<List<StandardSummary>>(request);
            }
        }

        public async Task<List<StandardSummary>> GetAllStandardSummaries()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/standards"))
            {
                return await RequestAndDeserialiseAsync<List<StandardSummary>>(request);
            }
        }

        public async Task<Provider> GetProvider(long providerUkPrn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/{providerUkPrn}"))
            {
                return await RequestAndDeserialiseAsync<Provider>(request);
            }
        } 
    }
}