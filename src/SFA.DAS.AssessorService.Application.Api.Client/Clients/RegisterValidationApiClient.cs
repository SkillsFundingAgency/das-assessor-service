using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class RegisterValidationApiClient : ApiClientBase, IRegisterValidationApiClient
    {
        public RegisterValidationApiClient(IAssessorApiClientFactory clientFactory, ILogger<RegisterValidationApiClient> logger) 
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<ValidationResponse> CreateEpaContactValidate(CreateEpaContactValidationRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/ao/assessment-organisations/contacts/validate-new"))
            {
                return await PostPutRequestWithResponseAsync<CreateEpaContactValidationRequest, ValidationResponse>(httpRequest, request);
            }
        }

        public async Task<ValidationResponse> CreateOrganisationValidate(CreateEpaOrganisationValidationRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/ao/assessment-organisations/validate-new"))
            {
                return await PostPutRequestWithResponseAsync<CreateEpaOrganisationValidationRequest, ValidationResponse>(httpRequest, request);
            }
        }

        public async Task<ValidationResponse> UpdateOrganisationValidate(UpdateEpaOrganisationValidationRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/ao/assessment-organisations/validate-existing"))
            {
                return await PostPutRequestWithResponseAsync<UpdateEpaOrganisationValidationRequest, ValidationResponse>(httpRequest, request);
            }
        }

        public async Task<ValidationResponse> CreateOrganisationStandardValidate(CreateEpaOrganisationStandardValidationRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/ao/assessment-organisations/standards/validate-new"))
            {
                return await PostPutRequestWithResponseAsync<CreateEpaOrganisationStandardValidationRequest, ValidationResponse>(httpRequest, request);
            }
        }
    }
}
