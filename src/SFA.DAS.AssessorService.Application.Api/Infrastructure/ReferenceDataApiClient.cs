using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Infrastructure
{
    public class ReferenceDataApiClient : IReferenceDataApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ReferenceDataApiClient> _logger;
        private readonly IWebConfiguration _config;

        public ReferenceDataApiClient(HttpClient client, ILogger<ReferenceDataApiClient> logger, IWebConfiguration configurationService)
        {
            _client = client;
            _logger = logger;
            _config = configurationService;
        }

        public async Task<IEnumerable<OrganisationSearchResult>> SearchOrgansiation(string searchTerm, bool exactMatch)
        {
            _logger.LogInformation($"Searching Reference Data API. Search Term: {searchTerm}");
            var apiResponse = await Get<IEnumerable<AssessorService.Api.Types.Models.ReferenceData.Organisation>>($"/api/organisations?searchTerm={searchTerm}");

            if (exactMatch)
            {
                apiResponse = apiResponse?.Where(r => r.Name.Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase)).AsEnumerable();
            }

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ReferenceData.Organisation>, IEnumerable<OrganisationSearchResult>>(apiResponse);
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        private string GetToken()
        {
            var tenantId = _config.ReferenceDataApiAuthentication.TenantId;
            var clientId = _config.ReferenceDataApiAuthentication.ClientId;
            var clientSecret = _config.ReferenceDataApiAuthentication.ClientSecret;
            var resourceId = _config.ReferenceDataApiAuthentication.ResourceId;

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;

            return result.AccessToken;
        }
    }
}
