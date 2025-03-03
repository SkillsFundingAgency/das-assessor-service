using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class FrameworkSearchApiClient : ApiClientBase, IFrameworkSearchApiClient
    {
        public FrameworkSearchApiClient(IAssessorApiClientFactory clientFactory, ILogger<SearchApiClient> logger) 
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<List<FrameworkSearchResult>> SearchFrameworks(FrameworkSearchQuery frameworkSearchQuery)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/search/frameworks"))
            {
                return await PostPutRequestWithResponseAsync<FrameworkSearchQuery, List<FrameworkSearchResult>>(request, frameworkSearchQuery);
            }
        }

        public async Task<GetFrameworkCertificateResult> GetFrameworkCertificate(Guid id)
        { 
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/search/framework-certificate/{id}"))
            {
                return await RequestAndDeserialiseAsync<GetFrameworkCertificateResult>(request,
                    $"Could not find the framework certificate");
            }
        }
    }
}