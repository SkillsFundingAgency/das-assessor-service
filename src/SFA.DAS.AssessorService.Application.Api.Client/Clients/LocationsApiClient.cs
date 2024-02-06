using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class LocationsApiClient : ApiClientBase, ILocationsApiClient
    {
        public LocationsApiClient(IAssessorApiClientFactory clientFactory, ILogger<ApiClientBase> logger)
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<List<AddressResponse>> SearchLocations(string query)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/locations?query={query}"))
            {
                return await RequestAndDeserialiseAsync<List<AddressResponse>>(request, $"Could not retrieve locations");
            }
        }
    }
}