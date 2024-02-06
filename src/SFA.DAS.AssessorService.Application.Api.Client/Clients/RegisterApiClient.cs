using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class RegisterApiClient : ApiClientBase, IRegisterApiClient
    {
        public RegisterApiClient(HttpClient httpClient, IAssessorTokenService tokenService, ILogger<ApiClientBase> logger)
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<List<AssessmentOrganisationSummary>> SearchOrganisations(string searchString)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ao/assessment-organisations/search/{searchString}"))
            {
                return await RequestAndDeserialiseAsync<List<AssessmentOrganisationSummary>>(request, $"Could not retrieve organisations search result");
            }
        }

        public async Task<EpaOrganisation> GetEpaOrganisation(string organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/assessment-organisations/{organisationId}"))
            {
                return await RequestAndDeserialiseAsync<EpaOrganisation>(request, $"Could not retrieve organisation search result");
            }
        }

        public async Task<AssessmentOrganisationContact> GetEpaContact(string contactId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/assessment-organisations/contacts/{contactId}"))
            {
                return await RequestAndDeserialiseAsync<AssessmentOrganisationContact>(request, $"Could not retrieve organisation contact search result");
            }
        }

        public async Task<EpaContact> GetEpaContactByEmail(string email)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/assessment-organisations/contacts/email/{email}"))
            {
                return await RequestAndDeserialiseAsync<EpaContact>(request, $"Could not retrieve organisation contact search result");
            }
        }

        public async Task<List<OrganisationStandardSummary>> GetEpaOrganisationStandards(string organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ao/assessment-organisations/{organisationId}/standards"))
            {
                return await RequestAndDeserialiseAsync<List<OrganisationStandardSummary>>(request, $"Could not retrieve organisation standard summary search result");
            }
        }

        public async Task<List<StandardVersion>> SearchStandards(string searchString)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ao/assessment-organisations/standards/search/{searchString}"))
            {
                return await RequestAndDeserialiseAsync<List<StandardVersion>>(request, $"Could not retrieve standard search result");
            }
        }

        public async Task<OrganisationStandard> GetOrganisationStandard(int organisationStandardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ao/assessment-organisations/organisation-standard/{organisationStandardId}"))
            {
                return await RequestAndDeserialiseAsync<OrganisationStandard>(request, $"Could not retrieve organisation standard search result");
            }
        }

        public async Task<string> CreateEpaOrganisation(CreateEpaOrganisationRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/ao/assessment-organisations"))
            {
                var result = await PostPutRequestWithResponseAsync<CreateEpaOrganisationRequest, EpaOrganisationResponse>(httpRequest, request);
                return result.Details;
            }
        }

        public async Task<string> UpdateEpaOrganisation(UpdateEpaOrganisationRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/ao/assessment-organisations"))
            {
                var result = await PostPutRequestWithResponseAsync<UpdateEpaOrganisationRequest, EpaOrganisationResponse>(httpRequest, request);
                return result.Details;
            }
        }

        public async Task<string> CreateEpaOrganisationStandard(CreateEpaOrganisationStandardRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/ao/assessment-organisations/standards"))
            {
                var result = await PostPutRequestWithResponseAsync<CreateEpaOrganisationStandardRequest, EpaoStandardResponse>(httpRequest, request);
                return result.Details;
            }
        }

        public async Task<string> UpdateEpaOrganisationStandard(UpdateEpaOrganisationStandardRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/ao/assessment-organisations/standards"))
            {
                var result = await PostPutRequestWithResponseAsync<UpdateEpaOrganisationStandardRequest, EpaoStandardResponse>(httpRequest, request);
                return result.Details;
            }
        }

        public async Task<string> CreateEpaContact(CreateEpaOrganisationContactRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/ao/assessment-organisations/contacts"))
            {
                var result = await PostPutRequestWithResponseAsync<CreateEpaOrganisationContactRequest, EpaOrganisationContactResponse>(httpRequest, request);
                return result.Details;
            }
        }

        public async Task<string> UpdateEpaContact(UpdateEpaOrganisationContactRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/ao/assessment-organisations/contacts"))
            {
                var result = await PostPutRequestWithResponseAsync<UpdateEpaOrganisationContactRequest, EpaOrganisationContactResponse>(httpRequest, request);
                return result.Details;
            }
        }

        public async Task<bool> AssociateOrganisationWithEpaContact(AssociateEpaOrganisationWithEpaContactRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/ao/assessment-organisations/contacts/associate-organisation"))
            {
                return await PostPutRequestWithResponseAsync<AssociateEpaOrganisationWithEpaContactRequest, bool>(httpRequest, request);
            }
        }

        public async Task<List<DeliveryArea>> GetDeliveryAreas()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/delivery-areas"))
            {
                return await RequestAndDeserialiseAsync<List<DeliveryArea>>(request, $"Could not retrieve delivery areas");
            }
        }
    }
}
