using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ApplicationApiClient : ApiClientBase, IApplicationApiClient
    {
        private readonly ILogger<ApplicationApiClient> _logger;

        public ApplicationApiClient(string baseUri, ITokenService tokenService, ILogger<ApplicationApiClient> logger) : base(baseUri, tokenService, logger)
        {
            _logger = logger;
        }

        public async Task<List<ApplicationResponse>> GetOrganisationApplications(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/organisation-applications"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve organsisation applications");
            }
        }

        public async Task<List<ApplicationResponse>> GetStandardApplications(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/standard-applications"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve standard applications");
            }
        }

        public async Task<List<ApplicationResponse>> GetWithdrawalApplications(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/withdrawal-applications"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve withdrawal applications");
            }
        }

        public async Task<List<ApplicationResponse>> GetOrganisationWithdrawalApplications(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/organisation-withdrawal-applications"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve organisation withdrawal applications");
            }
        }

        public async Task<List<ApplicationResponse>> GetStandardWithdrawalApplications(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/standard-withdrawal-applications"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve standard withdrawal applications");
            }
        }

        public async Task<ApplicationResponse> GetApplication(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{id}/application"))
            {
                return await RequestAndDeserialiseAsync<ApplicationResponse>(request, $"Could not retrieve applications");
            }
        }

        public async Task<ApplicationResponse> GetApplicationForUser(Guid id, Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/user/{userId}/application/{id}"))
            {
                return await RequestAndDeserialiseAsync<ApplicationResponse>(request, $"Could not retrieve application {id} for user {userId}");
            }
        }
        public async Task<ApplicationResponse> GetLastWithdrawnApplication(Guid orgId, int? standardCode)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{orgId}/application/withdrawn-old/{standardCode}"))
                {
                    return await RequestAndDeserialiseAsync<ApplicationResponse>(request, $"Could not retrieve withdrawn applications");
                }
            }
            catch
            {
                return new ApplicationResponse();
            }
        }

        public async Task<List<ApplicationResponse>> GetAllWithdrawnApplicationsForStandard(Guid orgId, int? standardCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{orgId}/application/withdrawn/{standardCode}"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve previous applications");
            }
        }


        public async Task<ApplicationResponse> GetPreviousApplication(Guid orgId, string standardReference)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{orgId}/application/previous/{standardReference}"))
                {
                    return await RequestAndDeserialiseAsync<ApplicationResponse>(request, $"Could not retrieve previous applications");
                }
            }
            catch
            {
                return new ApplicationResponse();
            }
        }

        public async Task<Guid> CreateApplication(CreateApplicationRequest createApplicationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/createApplication"))
            {
                return await PostPutRequestWithResponse<CreateApplicationRequest, Guid>(request, createApplicationRequest);
            }
        }

        public async Task DeleteApplications(DeleteApplicationsRequest deleteApplicationsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/deleteApplications"))
            {
                await PostPutRequest<DeleteApplicationsRequest>(request, deleteApplicationsRequest);
            }
        }

        public async Task<bool> SubmitApplicationSequence(SubmitApplicationSequenceRequest submitApplicationRequest)
        {

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/submitApplicationSequence"))
            {
                return await PostPutRequestWithResponse<SubmitApplicationSequenceRequest, bool>(request, submitApplicationRequest);
            }
        }

        public async Task<bool> UpdateStandardData(Guid Id, int standardCode, string referenceNumber, string standardName, List<string> versions, string standardApplicationType = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/updateStandardData"))
            {
                return await PostPutRequestWithResponse<UpdateStandardDataRequest, bool>(request, new UpdateStandardDataRequest
                {
                    Id = Id,
                    StandardCode = standardCode,
                    ReferenceNumber = referenceNumber,
                    StandardName = standardName,
                    Versions = versions,
                    StandardApplicationType = standardApplicationType
                });
            }
        }

        public async Task<bool> ResetApplicationToStage1(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/resetApplicationToStage1"))
            {
                return await PostPutRequestWithResponse<ResetApplicationToStage1Request, bool>(request, new ResetApplicationToStage1Request
                {
                    Id = id
                });
            }
        }

        public async Task<List<DeliveryArea>> GetQuestionDataFedOptions()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/delivery-areas"))
            {
                return await RequestAndDeserialiseAsync<List<DeliveryArea>>(request, $"Could not retrieve applications");
            }
        }

    }
}
