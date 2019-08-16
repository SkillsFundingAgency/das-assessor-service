using Microsoft.Extensions.Logging;
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
    }
}
