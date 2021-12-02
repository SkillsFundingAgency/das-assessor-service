using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class LocationsApiClient : ApiClientBase, ILocationsApiClient
    {
        public LocationsApiClient(string baseUri, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(baseUri, tokenService, logger)
        {
        }

        public LocationsApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
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