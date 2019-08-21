using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class QnaApiClient : ApiClientBase,IQnaApiClient
    {
        private readonly ILogger<QnaApiClient> _logger;

        public QnaApiClient(string baseUri, ITokenService tokenService, ILogger<QnaApiClient> logger) : base(baseUri, tokenService, logger)
        {
            _logger = logger;
        }

        public async Task<StartApplicationResponse> StartApplications(StartApplicationRequest startAppRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/start"))
            {
               return await PostPutRequestWithResponse<StartApplicationRequest,StartApplicationResponse>(request, startAppRequest);
            }
        }

        public async Task<ApplicationData> GetApplicationData(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/applicationData"))
            {
                return await RequestAndDeserialiseAsync<ApplicationData>(request,
                    $"Could not find the application");
            }
        }

        public async Task<Sequence> GetApplicationActiveSequence(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/current"))
            {
                return await RequestAndDeserialiseAsync<Sequence>(request,
                    $"Could not find the sequence");
            }
        }


        public async Task<List<Section>> GetSections(Guid applicationId, Guid sequenceId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceId}/sections"))
            {
                return await RequestAndDeserialiseAsync<List<Section>>(request,
                    $"Could not find the sections");
            }
        }

        public async Task<Section> GetSection(Guid applicationId, Guid sectionId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sections/{sectionId}"))
            {
                return await RequestAndDeserialiseAsync<Section>(request,
                    $"Could not find the section");
            }
        }
    }
}
