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

        /// <summary>
        /// Get a single organisation details
        /// GET /assessmentorgs/{organisationId}
        /// </summary>
        /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>a organisation details based on id</returns>
        public Organisation Get(string organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/assessment-organisations/{organisationId}"))
            {
                return RequestAndDeserialise<Organisation>(request, $"Could not find the organisation {organisationId}");
            }
        }

        /// <summary>
        /// Get a single organisation details
        /// GET /assessmentorgs/{organisationId}
        /// </summary>
        /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>a organisation details based on id</returns>
        public async Task<Organisation> GetAsync(string organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/assessment-organisations/{organisationId}"))
            {
                return await RequestAndDeserialiseAsync<Organisation>(request, $"Could not find the organisation {organisationId}");
            }
        }

        /// <summary>
        /// Get a collection of organisations
        /// GET /frameworks
        /// </summary>
        /// <returns>a collection of organisation summaries</returns>
        public IEnumerable<OrganisationSummary> FindAll()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/assessment-organisations"))
            {
                return RequestAndDeserialise<IEnumerable<OrganisationSummary>>(request);
            }
        }

        /// <summary>
        /// Get a collection of organisations
        /// GET /frameworks
        /// </summary>
        /// <returns>a collection of organisation summaries</returns>
        public async Task<IEnumerable<OrganisationSummary>> FindAllAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/assessment-organisations"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<OrganisationSummary>>(request);
            }
        }

        /// <summary>
        /// Get a collection of organisations
        /// GET /assessment-organisations/standards/{standardId}
        /// </summary>
        /// <param name="standardId">an integer for the standard id</param>
        /// <returns>a collection of organisation</returns>
        public IEnumerable<Organisation> ByStandard(int standardId)
        {
            return ByStandard(standardId.ToString());
        }

        /// <summary>
        /// Get a collection of organisations
        /// GET /assessment-organisations/standards/{standardId}
        /// </summary>
        /// <param name="standardId">an integer for the standard id</param>
        /// <returns>a collection of organisation</returns>
        public async Task<IEnumerable<Organisation>> ByStandardAsync(int standardId)
        {
            return await ByStandardAsync(standardId.ToString());
        }

        /// <summary>
        /// Get a collection of organisations
        /// GET /assessment-organisations/standards/{standardId}
        /// </summary>
        /// <param name="standardId">a string for the standard id</param>
        /// <returns>a collection of organisation</returns>
        public IEnumerable<Organisation> ByStandard(string standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/assessment-organisations/standards/{standardId}"))
            {
                return RequestAndDeserialise<IEnumerable<Organisation>>(request);
            }
        }

        /// <summary>
        /// Get a collection of organisations
        /// GET /assessment-organisations/standards/{standardId}
        /// </summary>
        /// <param name="standardId">a string for the standard id</param>
        /// <returns>a collection of organisation</returns>
        public async Task<IEnumerable<Organisation>> ByStandardAsync(string standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/assessment-organisations/standards/{standardId}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<Organisation>>(request);
            }
        }

        /// <summary>
        /// Check if a assessment organisation exists
        /// HEAD /assessmentorgs/{organisationId}
        /// </summary>
        /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>bool</returns>
        public bool Exists(string organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/assessment-organisations/{organisationId}"))
            {
                return Exists(request);
            }
        }

        /// <summary>
        /// Check if a assessment organisation exists
        /// HEAD /assessmentorgs/{organisationId}
        /// </summary>
        /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>bool</returns>
        public async Task<bool> ExistsAsync(string organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/assessment-organisations/{organisationId}"))
            {
                return await ExistsAsync(request);
            }
        }

        /// <summary>
        /// Get a collection of standards
        /// GET /assessment-organisations/{organisationId}/standards
        /// </summary>
        /// /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>a collection of standards</returns>
        public IEnumerable<StandardOrganisationSummary> FindAllStandardsByOrganisationId(string organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/assessment-organisations/{organisationId}/standards"))
            {
                return RequestAndDeserialise<IEnumerable<StandardOrganisationSummary>>(request);
            }
        }

        /// <summary>
        /// Get a collection of standards
        /// GET /assessment-organisations/{organisationId}/standards
        /// </summary>
        /// /// <param name="organisationId">a string for the organisation id</param>
        /// <returns>a collection of standards</returns>
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

        public async Task<List<Provider>> GetProviders()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers"))
            {
                return await RequestAndDeserialiseAsync<List<Provider>>(request);
            }
        }
    }
}