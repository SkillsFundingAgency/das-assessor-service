using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ApplicationApiClient : ApiClientBase, IApplicationApiClient
    {
        private readonly ILogger<ApplicationApiClient> _logger;

        public ApplicationApiClient(string baseUri, ITokenService tokenService, ILogger<ApplicationApiClient> logger) : base(baseUri, tokenService, logger)
        {
            _logger = logger;
        }

        public async Task<List<ApplicationResponse>> GetApplications(Guid userId, bool createdBy)
        {
            if (!createdBy)
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/Organisation"))
                {
                    return await RequestAndDeserialiseAsync <List<ApplicationResponse>>(request, $"Could not retrieve applications");
                }
            }
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/Applications/{userId}"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve applications");
            }
        }

        public async Task<ApplicationResponse> GetApplication(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{id}/application"))
            {
                return await RequestAndDeserialiseAsync<ApplicationResponse>(request, $"Could not retrieve applications");
            }
        }
    
        public async Task<Guid> CreateApplication(CreateApplicationRequest createApplicationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/createApplication"))
            {
                return await PostPutRequestWithResponse<CreateApplicationRequest, Guid>(request, createApplicationRequest);
            }
        }

        public async Task<List<Option>> GetQuestionDataFedOptions()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/delivery-areas"))
            {
                return (await RequestAndDeserialiseAsync<IEnumerable<DeliveryArea>>(request, $"Could not retrieve applications")).Select(da => new Option() { Label = da.Area, Value = da.Area }).ToList(); 
            }
        }

        public async Task<bool> Submit(Guid id, Guid userId, string email, Sequence sequence, List<Section> sections, string referenceFormat)
        {
            var applySections = sections.Select(x => new ApplySection
            {
                SectionId = x.Id,
                SectionNo = x.SectionNo,
                Status = ApplicationSectionStatus.Submitted,
            }).ToList();

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/submitApplication"))
            {
                return await PostPutRequestWithResponse<SubmitApplicationRequest, bool>(request, new SubmitApplicationRequest {
                    ApplicationId = id,
                    ReferenceFormat = referenceFormat,
                    Email = email,
                    UserId = userId,
                    Sequence = new ApplySequence
                    {
                        SequenceId = sequence.Id,
                        Sections  = applySections,
                        Status = ApplicationSequenceStatus.Submitted,
                        IsActive = sequence.IsActive,
                        SequenceNo = sequence.SequenceNo,
                        NotRequired = sequence.NotRequired
                    }
                });
            }
        }
    }
}
