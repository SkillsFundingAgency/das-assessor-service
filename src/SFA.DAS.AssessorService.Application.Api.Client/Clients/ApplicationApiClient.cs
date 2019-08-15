using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

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
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve applicaitons");
            }
        }
    }
}
