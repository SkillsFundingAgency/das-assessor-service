using System;
using SFA.DAS.AssessorService.Web.Services;

namespace SFA.DAS.AssessorService.ApiClientServices
{
    public class OrganisationApiService : IOrganisationService
    {
        private readonly IHttpClient _httpClient;
        private readonly IWebConfiguration _config;
        private readonly string _remoteServiceBaseUrl;

        public OrganisationService(IHttpClient httpClient, IWebConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            var apiServiceHost = _config.Api.ApiBaseAddress;
            _remoteServiceBaseUrl = $"{apiServiceHost}/api/v1/assessment-providers";
        }

        public async Task<Organisation> GetOrganisation(string token, int ukprn)
        {
            var apiUri = ApiUriGenerator.Organisation.GetOrganisation(_remoteServiceBaseUrl, ukprn);

            var getResponse = await _httpClient.GetAsync(apiUri, token);

            if (!getResponse.IsSuccessStatusCode) return null;

            var response = JsonConvert.DeserializeObject<Organisation>(await getResponse.Content.ReadAsStringAsync());

            return response;
        }
    }
}
